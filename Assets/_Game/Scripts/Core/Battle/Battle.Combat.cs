using System;
using System.Collections.Generic;
using Gacha.Core.Data;

namespace Gacha.Core.Battle
{
    public enum HitKind { Basic, Skill, Ult, Dot, Reflect, Curse, Proc }

    public sealed class HitInfo
    {
        public HitKind Kind;
        public string Elem, Ability;
        public bool CanCrit, Projectile, Landed, Crit;
        public double IgnoreDef;
        public bool Direct => Kind == HitKind.Basic || Kind == HitKind.Skill || Kind == HitKind.Ult;
    }

    /// <summary>How one hit's damage was built, factor by factor (filled only when Battle.OnHit is set).</summary>
    public sealed class HitTrace
    {
        public int Src, Dst; public string Ability; public HitKind Kind;
        public double Raw, AtkBuff = 1, Out = 1, Counter = 1, Crit = 1, Def = 1, In = 1, Shield = 1, Block = 1, Final;
        /// <summary>Everything stacked on top of the ability's nominal value at base ATK.</summary>
        public double Amplification => AtkBuff * Out * Counter * Crit * In * Shield;
        public override string ToString() =>
            $"x{Amplification:0.00} = ATK buffs {AtkBuff:0.00} · passives {Out:0.00} · race {Counter:0.00} · crit {Crit:0.00} · taken {In:0.00} · vs shield {Shield:0.00} (then DEF {Def:0.00}, block {Block:0.00})";
    }

    public sealed partial class Battle
    {
        /// <summary>Optional hook: receives a factor-by-factor trace of every landed hit (interaction audits).</summary>
        public Action<HitTrace> OnHit;

        // ---------------------------------------------------------------- status vocabulary

        public static readonly HashSet<string> Buffs = new HashSet<string> {
            "atk_up", "def_up", "haste_up", "aspd_up", "crit_up", "lifesteal_up", "unkillable", "immunity", "cc_immunity", "untargetable",
            "stealth", "regen", "hot", "counter", "reflect", "skill_shield", "hit_shield", "dr", "maxhp_up", "projectile_block", "redirect",
            "share", "bond_heal", "thread", "revive_ready", "revive_guard", "seed", "sure_crit", "dragonform", "halo", "planted", "thorns" };
        public static readonly HashSet<string> Debuffs = new HashSet<string> {
            "def_down", "atk_down", "dmg_down", "weaken", "mark", "star_mark", "slow", "blind", "antiheal", "silence", "burn", "bleed", "poison",
            "curse", "feeding_curse", "soaked", "stun", "sleep", "root", "fear", "charm", "taunt", "airborne", "stasis", "drained" };
        static readonly HashSet<string> Dots = new HashSet<string> { "burn", "bleed", "poison" };
        /// <summary>Control effects that control immunity blocks (combat.json effects "Control immunity").</summary>
        static readonly HashSet<string> ControlSet = new HashSet<string> { "stun", "sleep", "root", "fear", "charm", "silence", "airborne", "stasis" };

        /// <summary>Temporarily credits events to the ability that applied a status (restores on dispose).</summary>
        Scope Credit(Status s) => new Scope(this, s.Ab);
        readonly struct Scope : IDisposable
        {
            readonly Battle _b; readonly string _prev;
            public Scope(Battle b, string ab) { _b = b; _prev = b._ab; if (ab != null) b._ab = ab; }
            public void Dispose() => _b._ab = _prev;
        }

        public bool IsHardControl(string id) => T.HardControl.Contains(id);
        int BuffCount(Unit u) { int c = 0; foreach (var s in u.Statuses) if (s.Delay <= 0 && Buffs.Contains(s.Id)) c++; return c; }
        int DebuffCount(Unit u) { int c = 0; foreach (var s in u.Statuses) if (s.Delay <= 0 && Debuffs.Contains(s.Id)) c++; return c; }

        // ---------------------------------------------------------------- damage

        /// <summary>Full damage pipeline for one hit. Returns damage dealt (shields included).</summary>
        public double Hit(Unit s, Unit t, double raw, HitInfo h)
        {
            if (!t.Alive || raw <= 0) return 0;
            if (t.Has("stasis")) { Emit(Ev.Absorb, s.Index, t.Index, "stasis", raw); return 0; }
            if (h.Projectile && t.Has("projectile_block")) { Emit(Ev.Absorb, s.Index, t.Index, "projectile", raw); return 0; }
            if (h.Kind == HitKind.Basic)
            {
                double blind = s.Max("blind");
                if (blind > 0 && Rng.Chance(blind)) { Emit(Ev.Miss, s.Index, t.Index, "blind", 0); return 0; }
                double dodge = t.Base.Dodge + ModSum("dodge", t, s, t) - s.Base.Accuracy;
                if (dodge > 0 && Rng.Chance(dodge)) { Emit(Ev.Miss, s.Index, t.Index, "dodge", 0); return 0; }
            }
            if ((h.Kind == HitKind.Skill || h.Kind == HitKind.Ult) && RemoveOne(t, "skill_shield")) { Emit(Ev.Absorb, s.Index, t.Index, "skill_shield", raw); return 0; }
            if (h.Direct && RemoveOne(t, "hit_shield")) { Emit(Ev.Absorb, s.Index, t.Index, "hit_shield", raw); return 0; }
            h.Landed = true;

            var tr = OnHit != null ? new HitTrace { Src = s.Index, Dst = t.Index, Ability = h.Ability, Kind = h.Kind, Raw = raw } : null;
            if (tr != null && s.IsHero) tr.AtkBuff = s.Atk / Math.Max(1e-9, s.Base.Atk);
            double fOut = (1 + ModSum("dmg_out", s, s, t, h)) * (1 - s.Max("dmg_down"));
            double amt = raw * fOut;
            double fCounter = 1;
            if (Data.Counters.TryGetValue(s.Core, out var beats) && beats == t.Core) { fCounter = 1 + Data.CounterBonus; amt *= fCounter; }

            double fCrit = 1;
            if (h.CanCrit)
            {
                double cr = s.Base.CritRate + s.Max("crit_up") + ModSum("crit_rate", s, s, t, h) + (t.Has("sleep") ? T.SleepCritBonus : 0);
                if (s.Has("sure_crit")) { cr = 1; RemoveAll(s, "sure_crit"); }
                if (Rng.Chance(cr)) { h.Crit = true; fCrit = s.Base.CritDmg + ModSum("crit_dmg", s, s, t, h); amt *= fCrit; }
            }
            double fDef = 1;
            if (h.Kind != HitKind.Dot && h.Kind != HitKind.Curse) { fDef = T.DefK / (T.DefK + t.DefStat * (1 - h.IgnoreDef)); amt *= fDef; }

            double inc = t.Max("weaken") + t.Max("mark") - t.Max("dr") - t.Max("halo") + ModSum("dmg_in", t, s, t, h);
            if (t.Has("soaked")) { if (h.Elem == "fire") inc += T.SoakFire; else if (h.Elem == "lightning") inc += T.SoakLightning; }
            double fIn = Math.Max(0.1, 1 + inc);
            amt *= fIn;
            double fShield = 1;
            if (t.ShieldTotal > 0) { fShield = 1 + ModSum("dmg_vs_shield", s, s, t, h); amt *= fShield; }
            double fBlock = 1;
            if (h.Direct && Rng.Chance(t.Base.Block + ModSum("block", t, s, t))) { fBlock = 0.5; amt *= 0.5; }
            if (tr != null)
            {
                tr.Out = fOut; tr.Counter = fCounter; tr.Crit = fCrit; tr.Def = fDef; tr.In = fIn; tr.Shield = fShield; tr.Block = fBlock; tr.Final = amt;
                OnHit(tr);
            }

            // Damage sharing: Isolde's redirect, Thessaly's soul tether.
            if (h.Kind != HitKind.Reflect)
            {
                foreach (var st in t.Statuses.ToArray())
                {
                    if (st.Id != "redirect" || st.Delay > 0) continue;
                    var guard = Units[st.Source];
                    if (!guard.Alive || guard == t) continue;
                    double part = amt * st.V; amt -= part;
                    using (Credit(st)) ApplyDamage(s, guard, part, false, new HitInfo { Kind = HitKind.Reflect, Ability = "redirect" });
                }
                var share = t.Get("share");
                if (share != null)
                {
                    var partners = Units.FindAll(x => x != t && x.Alive && x.Statuses.Exists(q => q.Id == "share" && q.Source == share.Source));
                    if (partners.Count > 0)
                    {
                        double part = amt * share.V; amt -= part;
                        foreach (var p in partners) ApplyDamage(s, p, part / partners.Count, false, new HitInfo { Kind = HitKind.Reflect, Ability = "share" });
                    }
                }
            }

            double dealt = ApplyDamage(s, t, amt, h.Crit, h);
            AfterHit(s, t, dealt, h);
            return dealt;
        }

        /// <summary>Shields, HP, unkillable, seed, energy from damage taken, death.</summary>
        double ApplyDamage(Unit s, Unit t, double amt, bool crit, HitInfo h)
        {
            if (!t.Alive || amt <= 0) return 0;
            double left = amt;
            for (int i = 0; i < t.Shields.Count && left > 0; i++)
            {
                var sh = t.Shields[i];
                double take = Math.Min(sh.Amount, left);
                sh.Amount -= take; left -= take;
                if (sh.Amount <= 1e-9 && sh.BreakHeal > 0)
                {
                    var src = Units[sh.Source];
                    Heal(src, t, sh.BreakHeal * src.Atk, false);
                }
            }
            t.Shields.RemoveAll(x => x.Amount <= 1e-9);
            double before = t.Hp;
            t.Hp -= left;
            if (t.Hp < 1 && t.Has("unkillable")) t.Hp = 1;
            double lost = Math.Max(0, before - Math.Max(0, t.Hp));
            Emit(Ev.Damage, s.Index, t.Index, h.Ability ?? h.Kind.ToString(), amt, crit, h.IgnoreDef);
            if (lost > 0 && t.IsHero) GainEnergy(t, ENERGY_PER_PCT * lost / t.MaxHp * 100, true);
            var seed = t.Get("seed");
            if (seed != null && t.Hp > 0 && t.HpPct < 0.3) using (Credit(seed)) { t.Statuses.Remove(seed); Heal(Units[seed.Source], t, seed.V * t.MaxHp, false); }
            if (t.Hp <= 0) Kill(t, s);
            return amt;
        }

        const double ENERGY_PER_PCT = 0.5;   // combat.json energy.perPctHpLost

        void AfterHit(Unit s, Unit t, double dealt, HitInfo h)
        {
            if (dealt <= 0) return;
            if (h.Direct)
            {
                if (t.Alive) RemoveAll(t, "sleep");
                double ls = s.Base.Lifesteal + s.Max("lifesteal_up");
                if (ls > 0 && s.Alive)
                {
                    Heal(s, s, dealt * ls * (1 + ModSum("lifesteal_heal", s, s, s)), false);
                    foreach (var a in AlliesOf(s, false, false)) FireTriggers(a, "ally_lifesteal", s, 0);
                }
                // Reflection family (no further reflection from these)
                double refl = t.Sum("reflect");
                if (s.IsHero && s.Def.IsMelee) refl += ModSum("reflect_melee", t, s, t);
                if (h.Kind == HitKind.Skill || h.Kind == HitKind.Ult) refl += ModSum("reflect_skill", t, s, t);
                if (refl > 0 && s.Alive) Hit(t, s, dealt * refl, new HitInfo { Kind = HitKind.Reflect, Ability = "reflect" });
                var thorns = t.Get("thorns");
                if (thorns != null && s.Alive) using (Credit(thorns)) Hit(Units[thorns.Source], s, thorns.Amount, new HitInfo { Kind = HitKind.Reflect, Ability = "thorns" });
                var counter = t.Get("counter");
                if (counter != null && h.Kind == HitKind.Basic && s.IsHero && s.Def.IsMelee && t.Alive && s.Alive)
                    Hit(t, s, t.Atk * counter.V, new HitInfo { Kind = HitKind.Reflect, Ability = "counter", CanCrit = true });
                var feed = t.Get("feeding_curse");
                if (feed != null && s.Alive) using (Credit(feed)) Heal(Units[feed.Source], s, dealt * feed.V, false);
                var star = t.Get("star_mark");
                if (star != null && s.IsHero && s.Team != t.Team) using (Credit(star)) GainEnergy(s, star.V, false, Units[star.Source]);
                if (s.Has("stealth")) RemoveAll(s, "stealth");
            }
            if (h.Kind != HitKind.Reflect && h.Kind != HitKind.Curse)
            {
                var thread = t.Get("thread");
                if (thread != null && s.Alive) using (Credit(thread)) Hit(Units[thread.Source], s, dealt * thread.V, new HitInfo { Kind = HitKind.Reflect, Ability = "thread" });
                var bond = t.Get("bond_heal");
                if (bond != null && t.Alive) using (Credit(bond)) Heal(Units[bond.Source], t, dealt * bond.V, false);
                foreach (var cursed in Units)
                {
                    if (cursed == t || !cursed.Alive || cursed.Team != t.Team) continue;
                    var c = cursed.Get("curse");
                    if (c != null) using (Credit(c)) Hit(Units[c.Source], cursed, dealt * c.V, new HitInfo { Kind = HitKind.Curse, Ability = "curse" });
                }
            }
            if (h.Direct || h.Kind == HitKind.Proc)
            {
                if (h.Kind != HitKind.Proc) FireTriggers(s, "hit", t, dealt);
                if (h.Crit && h.Kind != HitKind.Proc) FireTriggers(s, "crit", t, dealt);
            }
        }

        // ---------------------------------------------------------------- heals, shields, energy

        public double Heal(Unit src, Unit t, double raw, bool fromSkill)
        {
            if (!t.Alive || raw <= 0) return 0;
            double mult = (1 + src.Base.HealPower + ModSum("heal_out", src, src, t)) * (1 + t.Base.HealRecv + ModSum("heal_in", t, src, t)) * (1 - Math.Min(1, t.Max("antiheal")));
            double amt = Math.Max(0, raw * mult);
            double real = Math.Min(amt, t.MaxHp - t.Hp);
            t.Hp += real;
            Emit(Ev.Heal, src.Index, t.Index, fromSkill ? "skill" : "passive", real);
            if (fromSkill)
            {
                src.LastHeal = amt;
                FireTriggers(src, "heal_given", t, amt);
                if (amt - real > 1e-6) FireTriggers(src, "overheal", t, amt - real);
            }
            return real;
        }

        public void AddShield(Unit src, Unit t, double raw, double breakHeal)
        {
            if (!t.Alive || raw <= 0) return;
            double amt = raw * (1 + src.Base.HealPower) * (1 + t.Base.HealRecv + ModSum("heal_in", t, src, t)) * (1 + t.ShieldRecv);
            double room = T.ShieldCap * t.MaxHp - t.ShieldTotal;
            amt = Math.Min(amt, Math.Max(0, room));
            if (amt <= 0) return;
            t.Shields.Add(new Shield { Amount = amt, Remaining = T.ShieldDuration, BreakHeal = breakHeal, Source = src.Index });
            Emit(Ev.ShieldGain, src.Index, t.Index, null, amt);
        }

        /// <summary>natural = earned by hitting / being hit / killing (scaled by energy gain); otherwise a gift or drain.</summary>
        public void GainEnergy(Unit u, double amount, bool natural, Unit from = null)
        {
            if (!u.Alive || u.IsSummon || amount == 0) return;
            if (natural) amount *= 1 + u.Base.EnergyRegen + ModSum("energy_gain", u, null, u) + HasteOf(u) * T.HasteEnergyFactor / 100.0;
            double before = u.Energy;
            u.Energy = Math.Max(0, Math.Min(100, u.Energy + amount));
            if (!natural) Emit(Ev.Energy, from?.Index ?? -1, u.Index, null, u.Energy - before);
            if (!natural && amount > 0 && from != null && from != u && from.Team == u.Team) FireTriggers(u, "energy_from_ally", EnemyTarget(u), amount);
        }

        // ---------------------------------------------------------------- statuses

        public bool ApplyStatus(Unit src, Unit t, string id, double dur, double v, Node node)
        {
            if (!t.Alive) return false;
            bool hostile = Debuffs.Contains(id) && src.Team != t.Team;
            if (hostile)
            {
                bool resist = t.Has("immunity")
                    || (id == "burn" && t.IsHero && t.Def.Passive.Flags.Contains("immune:burn"))
                    || (ControlSet.Contains(id) && (CcImmune(t, id) || ModSum("cc_immune", t, src, t) > 0));
                if (!resist)
                {
                    double chance = Math.Max(ResistFloor, t.Base.EffectRes - src.Base.EffectHit);
                    resist = Rng.Chance(chance);
                }
                if (resist) { Emit(Ev.Resist, src.Index, t.Index, id, 0); return false; }
            }
            if (IsHardControl(id))
            {
                dur = Math.Min(dur, T.ControlMax);
                int pri = T.HardControl.IndexOf(id);
                foreach (var s in t.Statuses)
                    if (s.Delay <= 0 && IsHardControl(s.Id) && T.HardControl.IndexOf(s.Id) < pri) { Emit(Ev.Resist, src.Index, t.Index, id, 0); return false; }
                if (hostile)
                {
                    if (Time - t.DrStart > T.DrWindow) { t.DrStart = Time; t.DrCount = 0; }
                    t.DrCount++;
                    if (t.DrCount == 2 && Time - t.DrStart <= T.DrHalfFor) dur *= 0.5;
                    else if (t.DrCount >= 2) { Emit(Ev.Resist, src.Index, t.Index, id, 0); return false; }
                }
                t.Statuses.RemoveAll(s => IsHardControl(s.Id) && s.Delay <= 0);
            }

            var st = new Status { Id = id, Remaining = dur, V = v, Source = src.Index, Delay = node?.Num("delay") ?? 0, Ab = _ab };
            if (node != null && node.Has("only")) st.Only = node.Strs("only");
            if (id == "burn" || id == "bleed") st.Amount = v * src.Atk;
            if (id == "thorns") st.Amount = v * src.Atk;

            if (Dots.Contains(id))
            {
                var same = t.Statuses.FindAll(s => s.Id == id);
                if (same.Count >= T.DotMaxStacks)
                {
                    Status oldest = same[0]; foreach (var s in same) if (s.Remaining < oldest.Remaining) oldest = s;
                    t.Statuses.Remove(oldest);
                }
                t.Statuses.Add(st);
            }
            else if (id == "redirect" || id == "share" || id == "thread" || id == "bond_heal")
            {
                t.Statuses.RemoveAll(s => s.Id == id && s.Source == src.Index);
                t.Statuses.Add(st);
            }
            else
            {
                var old = t.Statuses.Find(s => s.Id == id && s.Delay <= 0 && st.Delay <= 0);
                if (old != null) { old.V = Math.Max(old.V, v); old.Remaining = Math.Max(old.Remaining, dur); old.Source = src.Index; if (st.Amount > old.Amount) old.Amount = st.Amount; }
                else t.Statuses.Add(st);
                if (id == "dragonform" && old == null) t.Hp += t.Base.Hp * (1 + t.BonusHpPct) * v;
            }
            Emit(Ev.StatusOn, src.Index, t.Index, id, dur, false, v);
            if (hostile) FireTriggers(src, "debuff_applied", t, 0);
            return true;
        }

        bool CcImmune(Unit t, string id)
        {
            foreach (var s in t.Statuses)
                if (s.Id == "cc_immunity" && s.Delay <= 0 && (s.Only == null || s.Only.Contains(id))) return true;
            return false;
        }

        bool RemoveOne(Unit u, string id)
        {
            int i = u.Statuses.FindIndex(s => s.Id == id && s.Delay <= 0);
            if (i < 0) return false;
            u.LastHad[id] = Time;
            u.Statuses.RemoveAt(i);
            Emit(Ev.StatusOff, -1, u.Index, id, 0);
            return true;
        }

        void RemoveAll(Unit u, string id) { while (RemoveOne(u, id)) { } }

        public int Cleanse(Unit src, Unit t, int n)
        {
            int removed = 0;
            // hard control first, then everything else, oldest first
            for (int pass = 0; pass < 2 && removed < n; pass++)
                for (int i = 0; i < t.Statuses.Count && removed < n; i++)
                {
                    var s = t.Statuses[i];
                    if (!Debuffs.Contains(s.Id) || (pass == 0 && !IsHardControl(s.Id))) continue;
                    t.Statuses.RemoveAt(i--); removed++;
                    Emit(Ev.Cleanse, src.Index, t.Index, s.Id, 0);
                }
            return removed;
        }

        public int Dispel(Unit src, Unit t, int n, bool steal)
        {
            int removed = 0;
            for (int i = 0; i < t.Statuses.Count && removed < n; i++)
            {
                var s = t.Statuses[i];
                if (!Buffs.Contains(s.Id) || s.Delay > 0) continue;
                t.Statuses.RemoveAt(i--); removed++;
                Emit(Ev.Dispel, src.Index, t.Index, s.Id, 0);
                if (steal) ApplyStatus(src, src, s.Id, s.Remaining, s.V, null);
                FireTriggers(src, "dispel_done", t, 1);
            }
            return removed;
        }

        // ---------------------------------------------------------------- death and revive

        public void Kill(Unit t, Unit killer)
        {
            if (!t.Alive) return;
            if (t.IsHero && !t.NoRevive)
            {
                var rr = t.Get("revive_ready");
                if (rr != null && !t.Revived) { t.Statuses.Remove(rr); Revive(Units[rr.Source], t, rr.V); return; }
                foreach (var a in AlliesOf(t, false, false))
                {
                    var g = a.Get("revive_guard");
                    if (g != null && !t.Revived) { a.Statuses.Remove(g); Revive(a, t, g.V); return; }
                }
            }
            if (killer != null && killer.IsHero && killer.Def.Passive.Flags.Contains("no_revive_on_kill")) t.NoRevive = true;
            t.Hp = 0;
            Emit(Ev.Death, killer?.Index ?? -1, t.Index, null, 0);
            if (t.IsHero)
            {
                foreach (var u in Units.ToArray())
                {
                    if (!u.Alive || u == t) continue;
                    FireTriggers(u, u.Team == t.Team ? "ally_death" : "enemy_death", t, 0);
                }
                if (killer != null && killer.Alive) { GainEnergy(killer, 20, true); FireTriggers(killer, "kill", t, 0); }
            }
            t.Alive = false;
            t.Statuses.Clear(); t.Shields.Clear();
            foreach (var u in Units) if (u.IsSummon && u.Owner == t.Index && u.Summon.Permanent && u.Alive) Kill(u, null);
        }

        public bool Revive(Unit src, Unit t, double pct)
        {
            if (t.IsSummon || t.NoRevive || t.Revived) return false;
            t.Revived = true;
            t.Alive = true;
            t.Statuses.Clear(); t.Shields.Clear();
            t.Hp = Math.Max(1, t.MaxHp * pct);
            t.Energy = 0;
            Emit(Ev.Revive, src.Index, t.Index, null, t.Hp);
            return true;
        }

        // ---------------------------------------------------------------- per-tick upkeep

        void TickStatuses(double dt)
        {
            _ab = "dot";
            foreach (var u in Units)
            {
                if (!u.Alive) continue;
                for (int i = 0; i < u.Statuses.Count; i++)
                {
                    var s = u.Statuses[i];
                    if (s.Delay > 0) { s.Delay -= dt; if (s.Delay <= 0) Emit(Ev.StatusOn, s.Source, u.Index, s.Id, s.Remaining, false, s.V); continue; }
                    var src = Units[s.Source];
                    _ab = s.Ab ?? "dot";
                    switch (s.Id)
                    {
                        case "burn": Hit(src, u, s.Amount * dt, new HitInfo { Kind = HitKind.Dot, Elem = "fire", Ability = "burn" }); break;
                        case "bleed": Hit(src, u, s.Amount * dt * (u.Moved ? T.BleedMovingMult : 1), new HitInfo { Kind = HitKind.Dot, Ability = "bleed" }); break;
                        case "poison": Hit(src, u, s.V * u.MaxHp * dt, new HitInfo { Kind = HitKind.Dot, Ability = "poison" }); break;
                        case "regen": Heal(src, u, s.V * u.MaxHp * dt, false); break;
                        case "hot": Heal(src, u, s.Amount * dt, false); break;
                    }
                    if (!u.Alive) break;
                }
                if (!u.Alive) continue;
                double regen = ModSum("regen", u, u, u);
                if (regen > 0) Heal(u, u, regen * u.MaxHp * dt, false);
                for (int i = u.Statuses.Count - 1; i >= 0; i--)
                {
                    var s = u.Statuses[i];
                    if (s.Delay > 0) continue;
                    u.LastHad[s.Id] = Time;
                    s.Remaining -= dt;
                    if (s.Remaining <= 1e-9)
                    {
                        u.Statuses.RemoveAt(i);
                        Emit(Ev.StatusOff, -1, u.Index, s.Id, 0);
                        if (s.Id == "dragonform") u.Hp = Math.Min(u.Hp, u.MaxHp);
                    }
                }
                for (int i = u.Shields.Count - 1; i >= 0; i--) { u.Shields[i].Remaining -= dt; if (u.Shields[i].Remaining <= 0) u.Shields.RemoveAt(i); }
                // max HP can shrink (Dragonform ends): keep shields within the cap
                double over = u.ShieldTotal - T.ShieldCap * u.MaxHp;
                for (int i = u.Shields.Count - 1; i >= 0 && over > 1e-9; i--)
                {
                    double cut = Math.Min(over, u.Shields[i].Amount);
                    u.Shields[i].Amount -= cut; over -= cut;
                    if (u.Shields[i].Amount <= 1e-9) u.Shields.RemoveAt(i);
                }
                if (u.Hp > u.MaxHp) u.Hp = u.MaxHp;
            }
        }

        void TickZones(double dt)
        {
            for (int i = 0; i < _zones.Count; i++)
            {
                var z = _zones[i];
                if (!z.Caster.Alive) { _zones.RemoveAt(i--); continue; }
                z.Timer -= dt;
                if (z.Timer <= 1e-9)
                {
                    z.Timer += z.Every;
                    _ab = z.Ability;
                    RunOps(z.Caster, z.Ops, new OpCtx { Ability = z.Ability, Zone = z, Primary = null, Kind = HitKind.Skill });
                }
                z.Remaining -= dt;
                if (z.Remaining <= 1e-9) _zones.RemoveAt(i--);
            }
        }
    }
}
