using System;
using System.Collections.Generic;
using Gacha.Core.Data;

namespace Gacha.Core.Battle
{
    /// <summary>One side's line-up. Cells are formation grid coordinates [col, row] (hook for the 11 formations).</summary>
    public sealed class TeamSetup
    {
        public List<string> Heroes = new List<string>();
        public List<int[]> Cells;          // null = battle.json default formation
        public double StatScale = 1.0;     // hook for level sync / stage difficulty
        public TeamSetup(params string[] heroes) { Heroes.AddRange(heroes); }
    }

    /// <summary>
    /// Real-time battle on a fixed timestep (decisions rows 16, 19, 26). Everyone acts every tick; turn order inside a
    /// tick alternates teams and rotates so neither side gets a first-move edge. Fully deterministic for a given seed.
    /// </summary>
    public sealed partial class Battle
    {
        public readonly GameData Data;
        public readonly BattleTuning T;
        public readonly Rng Rng;
        public readonly List<Unit> Units = new List<Unit>();
        public readonly List<BattleEvent> Log = new List<BattleEvent>();
        public bool KeepLog = true;
        /// <summary>Minimum chance any debuff is resisted (combat.json: 15%). Tests may set 0 for deterministic checks.</summary>
        public double ResistFloor;
        /// <summary>Test switch: passives off (no mods, no triggers) so ability numbers can be measured cleanly.</summary>
        public bool DisablePassives;
        string _ab;
        public double Time { get; private set; }
        public bool Over { get; private set; }
        public int Winner { get; private set; } = -1;
        ulong _hash = 14695981039346656037UL;
        int _tickCount;

        /// <summary>Optional hook called for every event as it happens (live HP is readable; used by reports).</summary>
        public Action<BattleEvent> OnEvent;

        /// <summary>Optional hook called after every tick (invariant tests).</summary>
        public Action<Battle> AfterTick;

        sealed class Zone { public Unit Caster; public double X, Y, R, Remaining, Every, Timer; public List<Node> Ops; public string Ability; }
        readonly List<Zone> _zones = new List<Zone>();

        public Battle(GameData data, TeamSetup a, TeamSetup b, ulong seed)
        {
            Data = data; T = data.Tuning; Rng = new Rng(seed); ResistFloor = T.ResistFloor;
            AddTeam(a, 0); AddTeam(b, 1);
            BuildModIndex();
        }

        void AddTeam(TeamSetup setup, int team)
        {
            var cells = setup.Cells ?? T.Formation;
            for (int i = 0; i < setup.Heroes.Count; i++)
            {
                if (!Data.Heroes.TryGetValue(setup.Heroes[i], out var def)) throw new ArgumentException("Unknown hero " + setup.Heroes[i]);
                var s = def.Stats.Clone();
                s.Hp *= setup.StatScale * T.HpScale; s.Atk *= setup.StatScale; s.Def *= setup.StatScale;
                var u = new Unit { Index = Units.Count, Team = team, Def = def, Id = def.Id, Base = s };
                var cell = cells[i % cells.Count];
                double side = team == 0 ? -1 : 1;
                u.X = side * T.ColX[Math.Min(cell[0], T.ColX.Length - 1)];
                u.Y = T.RowY[Math.Min(cell[1], T.RowY.Length - 1)];
                u.Hp = u.MaxHp;
                u.Cd[0] = def.S1.Cooldown * T.InitialCooldownPct;
                u.Cd[1] = def.S2.Cooldown * T.InitialCooldownPct;
                u.HpHistory = new double[(int)Math.Round(3.0 / T.Tick) + 1];
                for (int k = 0; k < u.HpHistory.Length; k++) u.HpHistory[k] = u.Hp;
                Units.Add(u);
            }
            ApplyTeamBonus(team);
        }

        void ApplyTeamBonus(int team)
        {
            var counts = new Dictionary<string, int>();
            foreach (var u in Units) if (u.Team == team) counts[u.Core] = counts.TryGetValue(u.Core, out var c) ? c + 1 : 1;
            Dictionary<string, double> bonus = null;
            var vals = new List<int>(counts.Values); vals.Sort(); vals.Reverse();
            if (vals.Count >= 1 && vals[0] >= 5) bonus = Data.BonusFiveOfOne;
            else if (vals.Count >= 2 && vals[0] == 3 && vals[1] == 2) bonus = Data.BonusThreePlusTwo;
            if (bonus == null) return;
            foreach (var u in Units)
            {
                if (u.Team != team) continue;
                u.BonusHpPct = bonus.TryGetValue("hp", out var h) ? h : 0;
                u.BonusAtkPct = bonus.TryGetValue("atk", out var a) ? a : 0;
                u.BonusDefPct = bonus.TryGetValue("def", out var d) ? d : 0;
                u.Hp = u.MaxHp;
            }
        }

        // ---------------------------------------------------------------- run loop

        public BattleResult Run()
        {
            while (!Over) Step();
            return Result();
        }

        public void RunFor(double seconds)
        {
            double end = Time + seconds - 1e-9;
            while (!Over && Time < end) Step();
        }

        public BattleResult Result() => new BattleResult { Winner = Winner, Time = Time, Hash = _hash, Log = Log };

        public void Step()
        {
            if (Over) return;
            if (_tickCount == 0) Start();
            double dt = T.Tick;
            Time = Math.Round((_tickCount + 1) * dt, 6);
            _tickCount++;

            foreach (var u in Units) u.Moved = false;
            TickStatuses(dt);
            TickZones(dt);

            // Interleave teams and rotate who goes first so the order gives no edge.
            var order = new List<Unit>(Units.Count);
            var a = Units.FindAll(x => x.Team == 0); var b = Units.FindAll(x => x.Team == 1);
            bool bFirst = (_tickCount & 1) == 1;
            for (int i = 0; i < Math.Max(a.Count, b.Count); i++)
            {
                if (bFirst) { if (i < b.Count) order.Add(b[i]); if (i < a.Count) order.Add(a[i]); }
                else { if (i < a.Count) order.Add(a[i]); if (i < b.Count) order.Add(b[i]); }
            }
            foreach (var u in order) if (u.Alive && !Over) Act(u, dt);

            foreach (var u in Units)
            {
                u.HpHistoryPos = (u.HpHistoryPos + 1) % u.HpHistory.Length;
                u.HpHistory[u.HpHistoryPos] = u.Alive ? u.Hp : 0;
            }
            CheckEnd();
            if (!Over && Time >= T.TimeLimit - 1e-9) End(-1);
            AfterTick?.Invoke(this);
        }

        void Start()
        {
            Emit(Ev.Start, -1, -1, null, 0);
            foreach (var u in Units.ToArray()) FireTriggers(u, "battle_start", null, 0);
        }

        void CheckEnd()
        {
            bool aAlive = false, bAlive = false;
            foreach (var u in Units) if (u.Alive && u.IsHero) { if (u.Team == 0) aAlive = true; else bAlive = true; }
            if (!aAlive || !bAlive) End(aAlive ? 0 : bAlive ? 1 : -1);
        }

        void End(int winner)
        {
            if (Over) return;
            Over = true; Winner = winner;
            Emit(Ev.End, -1, -1, null, winner);
        }

        // ---------------------------------------------------------------- per-unit behaviour

        static readonly string[] Disabling = { "stasis", "stun", "airborne", "sleep" };

        void Act(Unit u, double dt)
        {
            if (u.IsSummon)
            {
                if (Time >= u.ExpiresAt || (u.Summon.Permanent && !Units[u.Owner].Alive)) { Kill(u, null); return; }
                if (u.Base.AtkSpd <= 0) return;   // decoy
            }
            double haste = HasteOf(u);
            for (int i = 0; i < 2; i++) if (u.Cd[i] > 0) u.Cd[i] = Math.Max(0, u.Cd[i] - dt * (1 + haste / 100.0));
            if (u.CastLock > 0) { u.CastLock -= dt; return; }
            foreach (var d in Disabling) if (u.Has(d)) return;

            if (u.Has("fear"))
            {
                var src = Units[u.Get("fear").Source];
                MoveAway(u, src, dt);
                return;
            }

            if (u.IsHero && !u.Has("charm"))
            {
                if (u.Energy >= 100 - 1e-9 && TryCast(u, u.Def.Ult)) return;
                if (!u.Has("silence"))
                {
                    if (u.Cd[0] <= 0 && TryCast(u, u.Def.S1)) { u.Cd[0] = u.Def.S1.Cooldown; return; }
                    if (u.Cd[1] <= 0 && TryCast(u, u.Def.S2)) { u.Cd[1] = u.Def.S2.Cooldown; return; }
                }
            }

            var target = EnemyTarget(u);
            u.LastTarget = target?.Index ?? -1;
            if (target == null) return;
            double reach = u.Base.Range + 0.25;
            if (Dist(u, target) <= reach)
            {
                u.AttackTimer -= dt;
                if (u.AttackTimer <= 0)
                {
                    BasicAttack(u, target);
                    u.AttackTimer += 1.0 / Math.Max(0.1, AttackSpeed(u));
                }
                return;
            }
            u.AttackTimer = Math.Min(u.AttackTimer, 0);
            MoveByMode(u, target, dt);
        }

        double AttackSpeed(Unit u) => u.Base.AtkSpd * Math.Max(0.1, 1 + u.Max("aspd_up") + ModSum("aspd", u, null, null) - u.Max("slow"));
        double MoveSpeed(Unit u) => u.Base.MoveSpd * Math.Max(0, 1 - u.Max("slow"));
        double HasteOf(Unit u) => u.Base.Haste + u.Max("haste_up") + ModSum("haste", u, null, null);

        void MoveByMode(Unit u, Unit target, double dt)
        {
            if (!CanMove(u)) return;
            string mode = u.IsSummon ? "advance" : u.Def.Move;
            if (mode == "dive" && !DiveOpen(u))
            {
                // hold just behind our own front line until it engages
                double fwd = u.Team == 0 ? 1 : -1, front = double.NegativeInfinity;
                foreach (var a in AlliesOf(u, false, false)) front = Math.Max(front, Forward(a));
                if (front > double.NegativeInfinity && Forward(u) < front - 1.0) MoveToward(u, (front - 1.0) * fwd, u.Y, dt);
                return;
            }
            if (mode == "follow" || mode == "guard")
            {
                var ally = mode == "guard" ? Pick(AlliesOf(u, false, true), x => -x.HpPct) : AllyTarget(u);
                if (ally != null && ally != u)
                {
                    double fwd = u.Team == 0 ? -1 : 1;    // stand a little behind the ally
                    double gx = ally.X + fwd * (mode == "guard" ? 0.6 : 1.5), gy = ally.Y;
                    if (DistTo(u, gx, gy) > 0.5) { MoveToward(u, gx, gy, dt); return; }
                    if (Dist(u, target) > u.Base.Range + 2.5) MoveToward(u, target.X, target.Y, dt);
                    return;
                }
            }
            MoveToward(u, target.X, target.Y, dt);
        }

        /// <summary>A diver goes in once any ally is in melee with an enemy, she has been hit, or after the opening seconds.</summary>
        bool DiveOpen(Unit u)
        {
            if (Time >= T.DiveDelay || u.Hp < u.MaxHp) return true;
            foreach (var a in AlliesOf(u, false, false))
                foreach (var e in EnemiesOf(u, true))
                    if (Dist(a, e) <= T.MeleeRangeMax) return true;
            return false;
        }

        bool CanMove(Unit u) => !u.Has("root") && !u.Has("planted") && u.Base.MoveSpd > 0;

        void MoveToward(Unit u, double x, double y, double dt)
        {
            double dx = x - u.X, dy = y - u.Y, d = Math.Sqrt(dx * dx + dy * dy);
            if (d < 1e-6) return;
            double step = Math.Min(d, MoveSpeed(u) * dt);
            u.X += dx / d * step; u.Y += dy / d * step;
            Separate(u);
            Clamp(u);
            u.Moved = step > 1e-6;
        }

        void MoveAway(Unit u, Unit from, double dt)
        {
            if (!CanMove(u)) return;
            double dx = u.X - from.X, dy = u.Y - from.Y, d = Math.Sqrt(dx * dx + dy * dy);
            if (d < 1e-6) { dx = u.Team == 0 ? -1 : 1; dy = 0; d = 1; }
            double step = MoveSpeed(u) * dt;
            u.X += dx / d * step; u.Y += dy / d * step;
            Clamp(u); u.Moved = true;
        }

        /// <summary>Keeps units on the same team from stacking on one point (deterministic nudge).</summary>
        void Separate(Unit u)
        {
            foreach (var o in Units)
            {
                if (o == u || !o.Alive || o.Team != u.Team) continue;
                double dx = u.X - o.X, dy = u.Y - o.Y, d = Math.Sqrt(dx * dx + dy * dy);
                if (d >= 0.6) continue;
                if (d < 1e-6) { dy = u.Index < o.Index ? -1 : 1; dx = 0; d = 1; }
                double push = (0.6 - d) * 0.5;
                u.X += dx / d * push; u.Y += dy / d * push;
            }
        }

        void Clamp(Unit u)
        {
            u.X = Math.Max(-T.HalfWidth, Math.Min(T.HalfWidth, u.X));
            u.Y = Math.Max(-T.HalfDepth, Math.Min(T.HalfDepth, u.Y));
        }

        /// <summary>Auto attack strength: the hero's own value if set, else the global one. Summons always use the global value.</summary>
        public double BasicPctOf(Unit u) => u.IsHero && u.Def.BasicPct >= 0 ? u.Def.BasicPct : T.BasicPct;

        void BasicAttack(Unit u, Unit target)
        {
            if (u.IsOff("basic")) return;
            _ab = "basic";
            if (u.Has("dragonform"))
            {
                foreach (var e in Around(target.X, target.Y, T.BreathRadius, EnemiesOf(u, true)))
                {
                    Hit(u, e, u.Atk * BasicPctOf(u), new HitInfo { Kind = HitKind.Basic, Elem = "fire", CanCrit = true, Ability = "basic" });
                    ApplyStatus(u, e, "burn", T.BreathBurnDur, T.BreathBurnV, null);
                }
                GainEnergy(u, 10, true);
                return;
            }
            var hit = new HitInfo { Kind = HitKind.Basic, CanCrit = true, Ability = "basic", Projectile = u.IsRanged };
            double dealt = Hit(u, target, u.Atk * BasicPctOf(u), hit);
            if (hit.Landed) GainEnergy(u, 10, true);
        }

        // ---------------------------------------------------------------- targeting (AI)

        public Unit EnemyTarget(Unit u)
        {
            var taunt = u.Get("taunt");
            if (taunt != null)
            {
                var t = Units[taunt.Source];
                if (t.Alive && Targetable(u, t)) return t;
            }
            if (u.Has("charm")) return Pick(AlliesOf(u, true, false).FindAll(x => x != u), x => -Dist(u, x));

            var cands = EnemiesOf(u, false);
            if (cands.Count == 0) return null;
            var marked = cands.FindAll(x => x.Has("mark"));
            if (marked.Count > 0) cands = marked;   // combat.json: marked enemies are preferred targets
            if (u.IsSummon) return Pick(cands, x => -Dist(u, x));

            string key = u.Def.EnemyAi, arg = null;
            int c = key.IndexOf(':'); if (c > 0) { arg = key.Substring(c + 1); key = key.Substring(0, c); }
            switch (key)
            {
                case "highest_atk": return Pick(cands, x => x.Atk);
                case "highest_hp": return Pick(cands, x => x.Hp);
                case "lowest_hp": return Pick(cands, x => -x.HpPct);
                case "most_energy": return Pick(cands, x => x.Energy);
                case "most_buffed": return Pick(cands, x => BuffCount(x) * 100 - Dist(u, x));
                case "densest": return Pick(cands, x => Around(x.X, x.Y, T.DensestR, cands).Count * 100 - Dist(u, x));
                case "healer_first":
                    {
                        var h = cands.FindAll(x => x.IsHero && x.Def.Role == "healer");
                        return Pick(h.Count > 0 ? h : cands, x => -Dist(u, x));
                    }
                case "near_weakest_ally":
                    {
                        var w = Pick(AlliesOf(u, false, true), x => -x.HpPct) ?? u;
                        return Pick(cands, x => -Dist(w, x));
                    }
                case "isolated_else_carry":
                    {
                        var iso = cands.FindAll(x => IsIsolated(x));
                        return iso.Count > 0 ? Pick(iso, x => -Dist(u, x)) : Pick(cands, x => x.Atk);
                    }
                case "lowest_hp_back":
                    {
                        var back = BackRow(cands);
                        return Pick(back.Count > 0 ? back : cands, x => -x.HpPct);
                    }
                case "nearest_with":
                    {
                        var w = cands.FindAll(x => x.Has(arg));
                        return Pick(w.Count > 0 ? w : cands, x => -Dist(u, x));
                    }
                case "with_else_highest_atk":
                    {
                        var w = cands.FindAll(x => x.Has(arg));
                        return w.Count > 0 ? Pick(w, x => -Dist(u, x)) : Pick(cands, x => x.Atk);
                    }
                case "random":
                    {
                        // sticky random: keep the same pick while it stays valid
                        if (u.Counters.TryGetValue("rand_target", out var idx) && idx < Units.Count && cands.Contains(Units[idx])) return Units[idx];
                        var r = cands[Rng.Range(0, cands.Count)];
                        u.Counters["rand_target"] = r.Index;
                        return r;
                    }
                default: return Pick(cands, x => -Dist(u, x));
            }
        }

        public Unit AllyTarget(Unit u)
        {
            if (u.IsSummon) return u;
            var allies = AlliesOf(u, false, true);
            var others = allies.FindAll(x => x != u);
            switch (u.Def.AllyAi)
            {
                case "lowest_hp": return Pick(allies, x => -x.HpPct);
                case "most_debuffed": return Pick(allies, x => DebuffCount(x) * 10 - x.HpPct);
                case "lost_most_recent": return Pick(allies, x => HpLostRecently(x) - x.HpPct * 0.001);
                case "closest_to_ult": return Pick(others.FindAll(x => x.Energy < 100), x => x.Energy) ?? Pick(others, x => x.Energy) ?? u;
                case "longest_cooldowns": return Pick(others, x => x.Cd[0] + x.Cd[1]) ?? u;
                case "assassin":
                    {
                        var a = others.FindAll(x => x.Def.Role == "dps" && x.Def.IsMelee);
                        return Pick(a.Count > 0 ? a : others, x => x.Atk) ?? u;
                    }
                case "fastest": return Pick(others, x => x.Base.MoveSpd * 100 + x.Atk * 0.001) ?? u;
                case "highest_atk": return Pick(others, x => x.Atk) ?? u;
                default: return u;
            }
        }

        double HpLostRecently(Unit x)
        {
            int back = (x.HpHistoryPos + 1) % x.HpHistory.Length;   // oldest entry = ~3s ago
            return Math.Max(0, x.HpHistory[back] - x.Hp) / Math.Max(1, x.MaxHp);
        }

        /// <summary>Highest score wins; ties go to the lowest unit index (deterministic).</summary>
        static Unit Pick(List<Unit> list, Func<Unit, double> score)
        {
            Unit best = null; double bs = double.NegativeInfinity;
            foreach (var x in list) { double s = score(x); if (s > bs + 1e-9) { bs = s; best = x; } }
            return best;
        }

        public bool Targetable(Unit by, Unit t)
        {
            if (!t.Alive || t.Untargetable || t.Has("untargetable")) return false;
            if (t.Has("stealth") && !(by.IsHero && by.Def.Passive.Flags.Contains("see_stealth"))) return false;
            return true;
        }

        /// <summary>Living enemies of u. aoe = true includes stealthed units (area effects still hit them).</summary>
        public List<Unit> EnemiesOf(Unit u, bool aoe)
        {
            var r = new List<Unit>();
            foreach (var x in Units)
            {
                if (!x.Alive || x.Team == u.Team) continue;
                if (aoe ? (x.Untargetable || x.Has("untargetable")) : !Targetable(u, x)) continue;
                r.Add(x);
            }
            return r;
        }

        public List<Unit> AlliesOf(Unit u, bool includeSummons, bool includeSelf)
        {
            var r = new List<Unit>();
            foreach (var x in Units)
            {
                if (!x.Alive || x.Team != u.Team) continue;
                if (!includeSelf && x == u) continue;
                if (x.IsSummon && !includeSummons) continue;
                r.Add(x);
            }
            return r;
        }

        /// <summary>How far toward the enemy a unit stands (team A faces +x, team B faces -x).</summary>
        double Forward(Unit x) => x.Team == 0 ? x.X : -x.X;

        List<Unit> FrontRow(List<Unit> team)
        {
            if (team.Count == 0) return team;
            double maxF = double.NegativeInfinity; foreach (var x in team) maxF = Math.Max(maxF, Forward(x));
            return team.FindAll(x => Forward(x) >= maxF - 1.5);
        }

        List<Unit> BackRow(List<Unit> team)
        {
            if (team.Count == 0) return team;
            var front = FrontRow(team);
            var back = team.FindAll(x => !front.Contains(x));
            if (back.Count > 0) return back;
            double minF = double.PositiveInfinity; foreach (var x in team) minF = Math.Min(minF, Forward(x));
            return team.FindAll(x => Forward(x) <= minF + 0.01);
        }

        bool IsIsolated(Unit x)
        {
            foreach (var o in Units) if (o != x && o.Alive && o.Team == x.Team && Dist(o, x) <= T.IsolatedR) return false;
            return true;
        }

        bool IsGrouped(Unit x)
        {
            foreach (var o in Units) if (o != x && o.Alive && o.Team == x.Team && Dist(o, x) <= T.GroupedR) return true;
            return false;
        }

        static List<Unit> Around(double x, double y, double r, List<Unit> from) => from.FindAll(u => DistTo(u, x, y) <= r + 1e-9);
        public static double Dist(Unit a, Unit b) => DistTo(a, b.X, b.Y);
        static double DistTo(Unit a, double x, double y) { double dx = a.X - x, dy = a.Y - y; return Math.Sqrt(dx * dx + dy * dy); }

        // ---------------------------------------------------------------- events

        internal void Emit(Ev type, int src, int dst, string what, double amount, bool crit = false, double v = 0)
        {
            unchecked
            {
                _hash = (_hash ^ (ulong)type) * 1099511628211UL;
                _hash = (_hash ^ (ulong)(src + 2)) * 1099511628211UL;
                _hash = (_hash ^ (ulong)(dst + 2)) * 1099511628211UL;
                if (what != null) foreach (char ch in what) _hash = (_hash ^ ch) * 1099511628211UL;
                _hash = (_hash ^ (ulong)(long)Math.Round(amount * 10)) * 1099511628211UL;
            }
            if (KeepLog || OnEvent != null)
            {
                var e = new BattleEvent { T = Time, Type = type, Src = src, Dst = dst, What = what, Amount = amount, Crit = crit, Ab = _ab, V = v };
                if (KeepLog) Log.Add(e);
                OnEvent?.Invoke(e);
            }
        }
    }
}
