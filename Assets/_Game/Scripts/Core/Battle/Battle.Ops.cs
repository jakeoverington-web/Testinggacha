using System;
using System.Collections.Generic;
using System.Globalization;
using Gacha.Core.Data;

namespace Gacha.Core.Battle
{
    /// <summary>Context for running a list of kit ops (from heroes.json "ops").</summary>
    public sealed class OpCtx
    {
        public string Ability;
        public HitKind Kind = HitKind.Skill;
        public Unit Primary, AllyPrimary, Evt, HitUnit;
        public double EvtAmount;
        internal object Zone;
    }

    /// <summary>Condition context: Self = owner of the passive or caster; Src/Tgt = the two sides of the event.</summary>
    public struct Cond { public Unit Self, Src, Tgt; public HitInfo Hit; }

    public sealed partial class Battle
    {
        // ---------------------------------------------------------------- vocabulary (validated against data by tests)

        public static readonly HashSet<string> OpNames = new HashSet<string> {
            "dmg", "heal", "shield", "status", "energy", "cleanse", "dispel", "move", "teleport", "zone", "summon", "revive",
            "cooldown", "chain", "rewind", "execute", "spend_hp", "counter" };
        public static readonly HashSet<string> Selectors = new HashSet<string> {
            "self", "target", "ally", "allies", "allies_except_self", "allies_near_self", "self_and_summons", "weakest_ally",
            "weakest_ally_other", "weakest_allies", "nearest_ally", "most_debuffed_ally", "most_debuffed_allies", "dead_ally",
            "enemies", "enemies_near_self", "enemies_near_target", "nearest_enemy", "nearest_enemies", "weakest_enemy", "weakest_enemies",
            "highest_atk_enemy", "highest_atk_enemies", "most_energy_enemy", "most_buffed_enemy", "backrow_enemy", "back_row", "front_row",
            "target_row", "line", "cone", "random_enemy_near_self", "weakest_with", "zone_enemies", "zone_allies", "evt", "evt_nearest_ally", "hit" };
        static readonly HashSet<string> SingleEnemySelectors = new HashSet<string> {
            "target", "nearest_enemy", "weakest_enemy", "highest_atk_enemy", "most_energy_enemy", "most_buffed_enemy", "backrow_enemy", "weakest_with", "evt" };
        public static readonly HashSet<string> ModKinds = new HashSet<string> {
            "dmg_out", "dmg_in", "crit_dmg", "crit_rate", "heal_out", "heal_in", "lifesteal_heal", "regen", "dodge", "block", "aspd", "haste",
            "energy_gain", "cc_immune", "reflect_melee", "reflect_skill", "dmg_vs_shield" };
        public static readonly HashSet<string> TriggerEvents = new HashSet<string> {
            "battle_start", "skill_cast", "ult_cast", "hit", "crit", "ally_death", "enemy_death", "kill", "heal_given", "overheal",
            "dispel_done", "debuff_applied", "ally_lifesteal", "energy_from_ally" };

        // ---------------------------------------------------------------- casting

        bool TryCast(Unit u, AbilityDef ab)
        {
            if (ab.Ops.Count == 0 || u.IsOff(ab.Key)) return false;
            if (ab.Requires == "dead_ally" && FindDeadAlly(u) == null) return false;
            var ctx = new OpCtx { Ability = ab.Key, Kind = ab.Key == "ult" ? HitKind.Ult : HitKind.Skill, Primary = EnemyTarget(u), AllyPrimary = AllyTarget(u) };
            var first = ab.Ops[0];
            string op = first.Str("op");
            if (op != "zone" && op != "summon" && op != "spend_hp" && op != "counter" && op != "rewind" && op != "cooldown")
            {
                string to = first.Str("to", "target");
                var targets = Select(u, to, ctx);
                if (targets.Count == 0) return false;
                if (to == "target" && Dist(u, ctx.Primary) > u.Base.Range + 0.25) return false;
                if (op == "heal" && !targets.Exists(x => x.HpPct < T.HealSkillThreshold)) return false;
                if (op == "cleanse" && !targets.Exists(x => DebuffCount(x) > 0)) return false;
                if (op == "dispel" && !targets.Exists(x => BuffCount(x) > 0)) return false;
                if (op == "revive" && targets.Count == 0) return false;
            }
            else if (op == "zone" && first.Str("at") == "target" && (ctx.Primary == null || Dist(u, ctx.Primary) > u.Base.Range + 2)) return false;
            else if (op == "rewind" && !AlliesOf(u, false, true).Exists(x => HpLostRecently(x) > 0.05)) return false;
            else if (op == "cooldown" && !AlliesOf(u, false, true).Exists(x => x.Cd[0] + x.Cd[1] > 2)) return false;

            DoCast(u, ab, ctx);
            return true;
        }

        void DoCast(Unit u, AbilityDef ab, OpCtx ctx)
        {
            _ab = ab.Key;
            Emit(Ev.Cast, u.Index, ctx.Primary?.Index ?? -1, ab.Key, 0);
            if (ab.Key == "ult") { Emit(Ev.Energy, u.Index, u.Index, "ult", -u.Energy); u.Energy = 0; }
            RunOps(u, ab.Ops, ctx);
            _ab = ab.Key;
            FireTriggers(u, ab.Key == "ult" ? "ult_cast" : "skill_cast", ctx.Primary, 0);
            u.CastLock = T.CastLock;
        }

        /// <summary>Test hook: cast an ability now, skipping cooldown, energy and "worth casting" checks.</summary>
        public void ForceCast(Unit u, string key)
        {
            var ab = key == "ult" ? u.Def.Ult : key == "s1" ? u.Def.S1 : u.Def.S2;
            DoCast(u, ab, new OpCtx { Ability = ab.Key, Kind = ab.Key == "ult" ? HitKind.Ult : HitKind.Skill, Primary = EnemyTarget(u), AllyPrimary = AllyTarget(u) });
        }

        int _depth;

        public void RunOps(Unit u, List<Node> ops, OpCtx ctx)
        {
            if (_depth > 4) return;
            _depth++;
            try { foreach (var n in ops) { if (Over) break; Exec(u, n, ctx); } }
            finally { _depth--; }
        }

        void Exec(Unit u, Node n, OpCtx ctx)
        {
            string op = n.Str("op");
            string to = n.Str("to", "target");
            switch (op)
            {
                case "dmg": OpDamage(u, n, ctx); return;
                case "chain": OpChain(u, n, ctx); return;
                case "zone": OpZone(u, n, ctx); return;
                case "summon": OpSummon(u, n); return;
                case "spend_hp": u.Hp = Math.Max(1, u.Hp - n.Num("pct") * u.MaxHp); Emit(Ev.Damage, u.Index, u.Index, "spend_hp", n.Num("pct") * u.MaxHp); return;
                case "counter": { string k = n.Str("key"); u.Counters[k] = (u.Counters.TryGetValue(k, out var c) ? c : 0) + (int)n.Num("add", 1); return; }
            }
            var targets = Select(u, to, ctx);
            foreach (var t in targets)
            {
                switch (op)
                {
                    case "heal":
                        {
                            double raw = n.Num("m") * u.Atk + n.Num("pct") * t.MaxHp + n.Num("lastHeal") * u.LastHeal;
                            if (n.Has("over"))
                            {
                                double over = n.Num("over");
                                if (ApplyStatus(u, t, "hot", over, raw / Math.Max(1, t.MaxHp), null)) { var hot = t.Get("hot"); hot.Amount = Math.Max(hot.Amount, raw / over); }
                                FireTriggers(u, "heal_given", t, raw);
                            }
                            else Heal(u, t, raw, true);
                            break;
                        }
                    case "shield":
                        {
                            double raw = n.Num("m") * u.Atk + n.Num("pct") * u.MaxHp + n.Num("pctTarget") * t.MaxHp + n.Num("fromEvent") * ctx.EvtAmount;
                            AddShield(u, t, raw, n.Num("breakHeal"));
                            break;
                        }
                    case "status":
                        {
                            string s = n.Str("s");
                            double dur = n.Has("dur") ? n.Num("dur") : T.DefaultDur(s, 3);
                            ApplyStatus(u, t, s, dur, n.Num("v"), n);
                            break;
                        }
                    case "energy":
                        {
                            double v = n.Num("v");
                            if (n.Has("fromOverheal")) v = ctx.EvtAmount / Math.Max(1, t.MaxHp) * 100 * n.Num("fromOverheal");
                            if (v < 0)
                            {
                                double drained = Math.Min(t.Energy, -v);
                                GainEnergy(t, -drained, false, u);
                                if (n.Bool("steal")) GainEnergy(u, drained, false, u);
                                if (n.Has("dmgPerEnergy") && drained > 0)
                                    Hit(u, t, drained * n.Num("dmgPerEnergy") * u.Atk, new HitInfo { Kind = ctx.Kind, Ability = ctx.Ability, CanCrit = ctx.Kind != HitKind.Proc });
                            }
                            else GainEnergy(t, v, false, u);
                            break;
                        }
                    case "cleanse": Cleanse(u, t, (int)n.Num("n", 99)); break;
                    case "dispel": Dispel(u, t, (int)n.Num("n", 99), n.Bool("steal")); break;
                    case "move": OpMove(u, t, n, ctx, targets); break;
                    case "teleport": OpTeleport(u, t, n); break;
                    case "revive": Revive(u, t, n.Num("pct", 0.4)); return;   // one revive per cast
                    case "cooldown":
                        {
                            double before = t.Cd[0] + t.Cd[1];
                            if (n.Bool("reset")) { t.Cd[0] = 0; t.Cd[1] = 0; }
                            else { double a = n.Num("advance"); t.Cd[0] = Math.Max(0, t.Cd[0] - a); t.Cd[1] = Math.Max(0, t.Cd[1] - a); }
                            Emit(Ev.Cooldown, u.Index, t.Index, null, before - t.Cd[0] - t.Cd[1]);
                        }
                        break;
                    case "rewind":
                        {
                            int back = (t.HpHistoryPos + 1) % t.HpHistory.Length;
                            double past = Math.Min(t.HpHistory[back], t.MaxHp);
                            if (past > t.Hp) { Emit(Ev.Heal, u.Index, t.Index, "rewind", past - t.Hp, false, n.Num("sec", 3)); t.Hp = past; }
                            break;
                        }
                    case "execute":
                        if (t.Alive && t.HpPct < n.Num("below"))
                        {
                            Emit(Ev.Damage, u.Index, t.Index, "execute", t.Hp, false, n.Num("below"));
                            Kill(t, u);
                            if (!t.Alive && n.Has("killEnergy")) GainEnergy(u, n.Num("killEnergy"), false, u);
                        }
                        break;
                }
                if (Over) return;
            }
        }

        void OpDamage(Unit u, Node n, OpCtx ctx)
        {
            int hits = (int)n.Num("hits", 1);
            string to = n.Str("to", "target");
            bool perHit = n.Bool("perHitTarget");
            var targets = perHit ? null : Select(u, to, ctx);
            bool projectile = u.IsRanged && SingleEnemySelectors.Contains(SelectorName(to)) && ctx.Zone == null;
            double total = 0;
            for (int i = 0; i < hits; i++)
            {
                if (perHit) targets = Select(u, to, ctx);
                foreach (var t in targets)
                {
                    if (!t.Alive) continue;
                    double raw = n.Num("m") * u.Atk + n.Num("pctMaxHp") * t.MaxHp;
                    var h = new HitInfo { Kind = ctx.Kind, Elem = n.Str("elem"), CanCrit = ctx.Kind != HitKind.Proc, IgnoreDef = n.Num("ignoreDef"), Projectile = projectile, Ability = ctx.Ability };
                    double dealt = Hit(u, t, raw, h);
                    total += dealt;
                    if (n.Has("drain") && dealt > 0) Heal(u, u, dealt * n.Num("drain"), false);
                    if (n.Has("execute") && t.Alive && t.HpPct < n.Num("execute")) { Emit(Ev.Damage, u.Index, t.Index, "execute", t.Hp, false, n.Num("execute")); Kill(t, u); }
                    if (n.Has("then")) { var sub = new OpCtx { Ability = ctx.Ability, Kind = ctx.Kind, Primary = ctx.Primary, AllyPrimary = ctx.AllyPrimary, HitUnit = t, Zone = ctx.Zone }; RunOps(u, n.Nodes("then"), sub); }
                    if (Over) return;
                }
            }
            if (n.Has("healTeam") && total > 0)
            {
                var team = AlliesOf(u, false, true);
                foreach (var a in team) Heal(u, a, total * n.Num("healTeam"), false);
            }
        }

        void OpChain(Unit u, Node n, OpCtx ctx)
        {
            var start = Select(u, n.Str("to", "target"), ctx);
            if (start.Count == 0) return;
            int bounces = (int)n.Num("bounces");
            string bonus = n.Str("bonusFrom");
            if (bonus != null && u.Counters.TryGetValue(bonus, out var extra)) { bounces += extra; u.Counters[bonus] = 0; }
            bool onlySoaked = n.Bool("onlySoaked"), twice = n.Bool("soakedDouble");
            var cur = start[0];
            var hitInfo = new Func<HitInfo>(() => new HitInfo { Kind = ctx.Kind, Elem = n.Str("elem"), CanCrit = ctx.Kind != HitKind.Proc, Ability = ctx.Ability });
            if (!n.Bool("skipFirst")) Hit(u, cur, n.Num("m") * u.Atk, hitInfo());
            var hitSet = new HashSet<Unit> { cur };
            for (int b = 0; b < bounces && !Over; b++)
            {
                var pool = EnemiesOf(u, true).FindAll(x => x != cur && Dist(x, cur) <= 4.0 && (!onlySoaked || x.Has("soaked")));
                if (pool.Count == 0) break;
                var fresh = pool.FindAll(x => !hitSet.Contains(x));
                var next = Pick(fresh.Count > 0 ? fresh : pool, x => -Dist(x, cur));
                int times = twice && cur.Has("soaked") && next.Has("soaked") ? 2 : 1;
                for (int k = 0; k < times; k++) Hit(u, next, n.Num("m") * u.Atk, hitInfo());
                hitSet.Add(next);
                cur = next;
            }
        }

        void OpZone(Unit u, Node n, OpCtx ctx)
        {
            double x = u.X, y = u.Y;
            if (n.Str("at") == "target" && ctx.Primary != null) { x = ctx.Primary.X; y = ctx.Primary.Y; }
            _zones.Add(new Zone { Caster = u, X = x, Y = y, R = n.Num("r", 2), Remaining = n.Num("dur", 3), Every = n.Num("every", 1), Timer = 0, Ops = n.Nodes("ops"), Ability = ctx.Ability });
            Emit(Ev.Zone, u.Index, -1, ctx.Ability, n.Num("dur", 3), false, n.Num("r", 2));
        }

        void OpSummon(Unit u, Node n)
        {
            if (!T.Summons.TryGetValue(n.Str("unit"), out var sd)) return;
            int count = (int)n.Num("n", 1);
            double fwd = u.Team == 0 ? 1 : -1;
            for (int i = 0; i < count; i++)
            {
                var s = new Unit
                {
                    Index = Units.Count, Team = u.Team, Def = u.Def, Id = sd.Id, IsSummon = true, Owner = u.Index, Summon = sd,
                    Base = new Stats { Hp = Math.Max(1, u.MaxHp * sd.HpPct), Atk = u.Atk * sd.AtkPct, Def = sd.Def, AtkSpd = sd.AtkSpd, Range = sd.Range, MoveSpd = sd.MoveSpd },
                    Untargetable = sd.Untargetable,
                    ExpiresAt = sd.Permanent ? double.MaxValue : Time + n.Num("dur", 10),
                    X = u.X + fwd * 0.8, Y = u.Y + (i - (count - 1) / 2.0) * 0.8
                };
                Clamp(s);
                s.Hp = s.MaxHp;
                s.HpHistory = new double[u.HpHistory.Length];
                for (int k = 0; k < s.HpHistory.Length; k++) s.HpHistory[k] = s.Hp;
                Units.Add(s);
                foreach (var m in sd.Mods) AddMod(s, m);
                Emit(Ev.Summon, u.Index, s.Index, sd.Id, s.Hp, false, sd.Permanent ? 0 : n.Num("dur", 10));
                if (n.Has("tauntR"))
                    foreach (var e in Around(s.X, s.Y, n.Num("tauntR"), EnemiesOf(u, true)))
                        ApplyStatus(s, e, "taunt", n.Num("dur", 3), 0, null);
            }
        }

        void OpMove(Unit u, Unit t, Node n, OpCtx ctx, List<Unit> all)
        {
            string kind = n.Str("kind");
            double dist = n.Num("dist");
            switch (kind)
            {
                case "push":
                    {
                        double dir = Math.Sign(t.X - u.X); if (dir == 0) dir = t.Team == 0 ? -1 : 1;
                        t.X += dir * dist; break;
                    }
                case "pull_to_caster":
                    {
                        double dir = Math.Sign(t.X - u.X); if (dir == 0) dir = u.Team == 0 ? 1 : -1;
                        t.X = u.X + dir * 1.0; t.Y = u.Y + ((t.Index % 3) - 1) * 0.4; break;
                    }
                case "pull_to_center":
                    {
                        double cx = 0, cy = 0; foreach (var a in all) { cx += a.X; cy += a.Y; }
                        cx /= all.Count; cy /= all.Count;
                        t.X += (cx - t.X) * 0.8; t.Y += (cy - t.Y) * 0.8; break;
                    }
                case "pull_to_point":
                    {
                        var z = ctx.Zone as Zone;
                        double px = z?.X ?? u.X, py = z?.Y ?? u.Y;
                        double dx = px - t.X, dy = py - t.Y, d = Math.Sqrt(dx * dx + dy * dy);
                        if (d > 1e-6) { double s = Math.Min(d, dist); t.X += dx / d * s; t.Y += dy / d * s; }
                        break;
                    }
            }
            t.Moved = true;
            Clamp(t);
            Emit(Ev.Move, u.Index, t.Index, kind, dist);
        }

        void OpTeleport(Unit u, Unit t, Node n)
        {
            if (n.Str("where") == "away")
            {
                u.X = (u.Team == 0 ? -1 : 1) * (T.HalfWidth - 1);
            }
            else
            {
                double dir = u.Team == 0 ? 1 : -1;     // land on the far side of the target
                u.X = t.X + dir * 0.8; u.Y = t.Y;
            }
            Clamp(u); u.Moved = true;
            Emit(Ev.Move, u.Index, u.Index, "teleport", 0);
        }

        // ---------------------------------------------------------------- selectors

        static string SelectorName(string to) { int c = to.IndexOf(':'); return c < 0 ? to : to.Substring(0, c); }

        public List<Unit> Select(Unit u, string to, OpCtx ctx)
        {
            string name = to, arg = null;
            int c = to.IndexOf(':');
            if (c > 0) { name = to.Substring(0, c); arg = to.Substring(c + 1); }
            double num = arg != null && double.TryParse(arg, NumberStyles.Float, CultureInfo.InvariantCulture, out var nv) ? nv : 0;
            var r = new List<Unit>();
            switch (name)
            {
                case "self": r.Add(u); return r;
                case "target": if (ctx.Primary != null && ctx.Primary.Alive) r.Add(ctx.Primary); return r;
                case "ally": { var a = ctx.AllyPrimary ?? AllyTarget(u); if (a != null && a.Alive) r.Add(a); return r; }
                case "allies": return AlliesOf(u, false, true);
                case "allies_except_self": return AlliesOf(u, false, false);
                case "allies_near_self": return AlliesOf(u, false, true).FindAll(x => Dist(u, x) <= num);
                case "self_and_summons": return AlliesOf(u, true, true).FindAll(x => x == u || (x.IsSummon && x.Owner == u.Index));
                case "weakest_ally": return One(Pick(AlliesOf(u, false, true), x => -x.HpPct));
                case "weakest_ally_other": return One(Pick(AlliesOf(u, false, false), x => -x.HpPct));
                case "weakest_allies": return Top(AlliesOf(u, false, true), x => -x.HpPct, (int)num);
                case "nearest_ally": return One(Pick(AlliesOf(u, false, false), x => -Dist(u, x)) ?? u);
                case "most_debuffed_ally": return One(Pick(AlliesOf(u, false, true), x => DebuffCount(x) * 10 - x.HpPct));
                case "most_debuffed_allies": return Top(AlliesOf(u, false, true), x => DebuffCount(x) * 10 - x.HpPct, (int)num);
                case "dead_ally": return One(FindDeadAlly(u));
                case "enemies": return EnemiesOf(u, true);
                case "enemies_near_self": return Around(u.X, u.Y, num, EnemiesOf(u, true));
                case "enemies_near_target": return ctx.Primary == null ? r : Around(ctx.Primary.X, ctx.Primary.Y, num, EnemiesOf(u, true));
                case "nearest_enemy": return One(Pick(EnemiesOf(u, false), x => -Dist(u, x)));
                case "nearest_enemies": return Top(EnemiesOf(u, false), x => -Dist(u, x), (int)num);
                case "weakest_enemy": return One(Pick(EnemiesOf(u, false), x => -x.HpPct));
                case "weakest_enemies": return Top(EnemiesOf(u, false), x => -x.HpPct, (int)num);
                case "highest_atk_enemy": return One(Pick(EnemiesOf(u, false), x => x.Atk));
                case "highest_atk_enemies": return Top(EnemiesOf(u, false), x => x.Atk, (int)num);
                case "most_energy_enemy": return One(Pick(EnemiesOf(u, false), x => x.Energy));
                case "most_buffed_enemy": return One(Pick(EnemiesOf(u, false), x => BuffCount(x)));
                case "backrow_enemy": { var e = EnemiesOf(u, false); return One(Pick(BackRow(e), x => -x.HpPct)); }
                case "back_row": return BackRow(EnemiesOf(u, true));
                case "front_row": return FrontRow(EnemiesOf(u, true));
                case "target_row":
                    {
                        if (ctx.Primary == null) return r;
                        var e = EnemiesOf(u, true); var front = FrontRow(e);
                        return front.Contains(ctx.Primary) ? front : BackRow(e);
                    }
                case "line": return Lane(u, ctx.Primary, 1.0);
                case "cone": return Lane(u, ctx.Primary, 2.0);
                case "random_enemy_near_self":
                    {
                        var e = Around(u.X, u.Y, num, EnemiesOf(u, true));
                        if (e.Count > 0) r.Add(e[Rng.Range(0, e.Count)]);
                        return r;
                    }
                case "weakest_with":
                    {
                        var ids = arg.Split('|');
                        var e = EnemiesOf(u, false);
                        var w = e.FindAll(x => Array.Exists(ids, id => x.Has(id)));
                        return One(Pick(w.Count > 0 ? w : e, x => -x.HpPct));
                    }
                case "zone_enemies": { var z = (Zone)ctx.Zone; return z == null ? r : Around(z.X, z.Y, z.R, EnemiesOf(u, true)); }
                case "zone_allies": { var z = (Zone)ctx.Zone; return z == null ? r : Around(z.X, z.Y, z.R, AlliesOf(u, false, true)); }
                case "evt": if (ctx.Evt != null && ctx.Evt.Alive) r.Add(ctx.Evt); return r;
                case "evt_nearest_ally":
                    {
                        if (ctx.Evt == null) return r;
                        var e = Units.FindAll(x => x.Alive && x != ctx.Evt && x.Team == ctx.Evt.Team && !x.Untargetable);
                        return One(Pick(e, x => -Dist(ctx.Evt, x)));
                    }
                case "hit": if (ctx.HitUnit != null && ctx.HitUnit.Alive) r.Add(ctx.HitUnit); return r;
            }
            throw new ArgumentException("Unknown selector " + to);
        }

        static List<Unit> One(Unit x) { var r = new List<Unit>(); if (x != null) r.Add(x); return r; }

        static List<Unit> Top(List<Unit> list, Func<Unit, double> score, int n)
        {
            var sorted = new List<Unit>(list);
            sorted.Sort((a, b) => { int c = score(b).CompareTo(score(a)); return c != 0 ? c : a.Index.CompareTo(b.Index); });
            if (sorted.Count > n) sorted.RemoveRange(n, sorted.Count - n);
            return sorted;
        }

        /// <summary>Enemies within `width` of the line from u through the target, out to 3 m past it.</summary>
        List<Unit> Lane(Unit u, Unit target, double width)
        {
            var r = new List<Unit>();
            if (target == null) return r;
            double dx = target.X - u.X, dy = target.Y - u.Y, len = Math.Sqrt(dx * dx + dy * dy);
            if (len < 1e-6) { r.Add(target); return r; }
            dx /= len; dy /= len;
            foreach (var e in EnemiesOf(u, true))
            {
                double px = e.X - u.X, py = e.Y - u.Y;
                double along = px * dx + py * dy, across = Math.Abs(px * dy - py * dx);
                if (along >= -0.5 && along <= len + 3 && across <= width) r.Add(e);
            }
            if (!r.Contains(target) && target.Alive) r.Add(target);
            return r;
        }

        Unit FindDeadAlly(Unit u)
        {
            foreach (var x in Units) if (!x.Alive && x.IsHero && x.Team == u.Team && !x.NoRevive && !x.Revived) return x;
            return null;
        }

        // ---------------------------------------------------------------- passives: mods and triggers

        readonly Dictionary<string, List<(Unit owner, ModDef mod)>> _mods = new Dictionary<string, List<(Unit, ModDef)>>();

        void BuildModIndex()
        {
            foreach (var u in Units) foreach (var m in u.Def.Passive.Mods) AddMod(u, m);
        }

        void AddMod(Unit owner, ModDef m)
        {
            if (!_mods.TryGetValue(m.K, out var l)) _mods[m.K] = l = new List<(Unit, ModDef)>();
            l.Add((owner, m));
        }

        /// <summary>Sum of passive modifiers of one kind that apply to `subject` (src/tgt feed the conditions).</summary>
        public double ModSum(string kind, Unit subject, Unit src, Unit tgt, HitInfo h = null)
        {
            if (DisablePassives) return 0;
            if (!_mods.TryGetValue(kind, out var list)) return 0;
            double sum = 0;
            foreach (var (owner, m) in list)
            {
                if (!owner.Alive || owner.IsOff("passive") || !InScope(owner, m.Scope, subject)) continue;
                if (m.If != null && !Eval(m.If, new Cond { Self = owner, Src = src, Tgt = tgt, Hit = h })) continue;
                double v = m.V;
                if (m.PerStack != null) { int st = 0; if (tgt != null) foreach (var s in tgt.Statuses) if (s.Id == m.PerStack && s.Source == owner.Index) st++; v *= st; }
                if (m.PerBuff > 0) v *= Math.Min(m.PerBuff, BuffCount(subject));
                sum += v;
            }
            return sum;
        }

        bool InScope(Unit owner, string scope, Unit subject)
        {
            if (subject == null) return false;
            string name = scope; double r = 0;
            int c = scope.IndexOf(':');
            if (c > 0) { name = scope.Substring(0, c); r = double.Parse(scope.Substring(c + 1), CultureInfo.InvariantCulture); }
            switch (name)
            {
                case "self": return subject == owner;
                case "allies": return subject.Team == owner.Team;
                case "allies_near": return subject.Team == owner.Team && Dist(owner, subject) <= r;
                case "allies_behind": return subject.Team == owner.Team && subject != owner && Forward(subject) < Forward(owner) - 0.3;
                case "enemies": return subject.Team != owner.Team;
            }
            return false;
        }

        void FireTriggers(Unit u, string ev, Unit evt, double amount)
        {
            if (DisablePassives) return;
            if (!u.Alive || !u.IsHero || u.IsOff("passive")) return;
            var trig = u.Def.Passive.Triggers;
            for (int i = 0; i < trig.Count; i++)
            {
                var t = trig[i];
                if (t.On != ev) continue;
                if (t.Icd > 0 && u.TriggerReadyAt.TryGetValue(i, out var ready) && Time < ready - 1e-9) continue;
                if (t.If != null && !Eval(t.If, new Cond { Self = u, Src = u, Tgt = evt })) continue;
                if (t.Icd > 0) u.TriggerReadyAt[i] = Time + t.Icd;
                string prev = _ab; _ab = "passive";
                RunOps(u, t.Ops, new OpCtx { Ability = "passive", Kind = HitKind.Proc, Primary = EnemyTarget(u), AllyPrimary = AllyTarget(u), Evt = evt, EvtAmount = amount });
                _ab = prev;
            }
        }

        // ---------------------------------------------------------------- conditions
        // Grammar: alternatives joined by '|'. Atom = [!]subject:status | [!]subject.prop[<|>value] | ult | melee.
        // An atom with no subject inherits the previous atom's subject ("tgt:blind|stun").

        internal sealed class Atom { public bool Neg; public string Subj, Status, Prop; public char Cmp; public double Val; }
        readonly Dictionary<string, Atom[]> _condCache = new Dictionary<string, Atom[]>();

        internal static Atom[] ParseCondition(string text)
        {
            var parts = text.Split('|');
            var atoms = new Atom[parts.Length];
            string lastSubj = null; bool lastIsStatus = true;
            for (int i = 0; i < parts.Length; i++)
            {
                string p = parts[i].Trim();
                var a = new Atom();
                if (p.StartsWith("!")) { a.Neg = true; p = p.Substring(1); }
                int colon = p.IndexOf(':'), dot = p.IndexOf('.');
                if (p == "ult" || p == "melee") { a.Prop = p; atoms[i] = a; continue; }
                if (colon > 0 && (dot < 0 || colon < dot)) { a.Subj = p.Substring(0, colon); a.Status = p.Substring(colon + 1); lastIsStatus = true; }
                else if (dot > 0) { a.Subj = p.Substring(0, dot); ParseProp(a, p.Substring(dot + 1)); lastIsStatus = false; }
                else if (lastSubj != null) { a.Subj = lastSubj; if (lastIsStatus) a.Status = p; else ParseProp(a, p); }
                else throw new FormatException("Bad condition atom: " + parts[i]);
                lastSubj = a.Subj;
                atoms[i] = a;
            }
            return atoms;
        }

        static void ParseProp(Atom a, string p)
        {
            int k = p.IndexOfAny(new[] { '<', '>' });
            if (k < 0) { a.Prop = p; return; }
            a.Prop = p.Substring(0, k); a.Cmp = p[k];
            a.Val = double.Parse(p.Substring(k + 1), CultureInfo.InvariantCulture);
        }

        /// <summary>Validates condition text (for data tests). Returns null when fine, else the problem.</summary>
        public static string CheckCondition(string text)
        {
            try
            {
                foreach (var a in ParseCondition(text))
                {
                    if (a.Subj != null && !CondSubjects.Contains(a.Subj)) return "unknown subject " + a.Subj;
                    if (a.Status != null && !Buffs.Contains(a.Status) && !Debuffs.Contains(a.Status)) return "unknown status " + a.Status;
                    if (a.Prop != null && !CondProps.Contains(a.Prop)) return "unknown property " + a.Prop;
                }
                return null;
            }
            catch (System.Exception e) { return e.Message; }
        }

        public static readonly HashSet<string> CondSubjects = new HashSet<string> { "tgt", "src", "self", "any_enemy" };
        public static readonly HashSet<string> CondProps = new HashSet<string> { "hp", "energy", "grouped", "isolated", "shielded", "buffed", "summon", "beast", "ult", "melee" };

        public bool Eval(string text, Cond c)
        {
            if (!_condCache.TryGetValue(text, out var atoms)) _condCache[text] = atoms = ParseCondition(text);
            foreach (var a in atoms) if (EvalAtom(a, c) != a.Neg) return true;
            return false;
        }

        bool EvalAtom(Atom a, Cond c)
        {
            if (a.Prop == "ult" && a.Subj == null) return c.Hit != null && c.Hit.Kind == HitKind.Ult;
            if (a.Prop == "melee" && a.Subj == null) return c.Src != null && c.Src.IsHero && c.Src.Def.IsMelee;
            if (a.Subj == "any_enemy")
            {
                foreach (var e in Units) if (e.Alive && e.Team != c.Self.Team && e.Has(a.Status)) return true;
                return false;
            }
            var x = a.Subj == "tgt" ? c.Tgt : a.Subj == "src" ? c.Src : c.Self;
            if (x == null) return false;
            if (a.Status != null) return x.Has(a.Status);
            switch (a.Prop)
            {
                case "hp": return a.Cmp == '<' ? x.HpPct < a.Val : x.HpPct > a.Val;
                case "energy": return a.Cmp == '<' ? x.Energy < a.Val : x.Energy > a.Val;
                case "grouped": return IsGrouped(x);
                case "isolated": return IsIsolated(x);
                case "shielded": return x.ShieldTotal > 0;
                case "buffed": return BuffCount(x) > 0;
                case "summon": return x.IsSummon;
                case "beast": return Units.Exists(s => s.Alive && s.IsSummon && s.Owner == x.Index && !s.Summon.Permanent && s.Id == "wolf");
            }
            return false;
        }
    }
}
