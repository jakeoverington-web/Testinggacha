using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Gacha.Core.Battle;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>
    /// Training ground: every hero against neutral dummies (DEF 0, no race, resist 0, crit off, passives off).
    /// Checks that auto attacks deal 100% ATK at the hero's attack speed, and that every number written in an
    /// ability's text (% ATK, % max HP, seconds, %, energy) shows up in what the ability actually did.
    /// </summary>
    [TestFixture]
    public class TrainingGroundTests
    {
        public static IEnumerable<string> HeroIds => InvariantTests.Roster;
        public static IEnumerable<object[]> Abilities => InvariantTests.Roster.SelectMany(h => new[] { "ult", "s1", "s2" }.Select(k => new object[] { h, k }));

        // ---------- auto attacks ----------

        [TestCaseSource(nameof(HeroIds))]
        public void AutoAttack_DealsBasicPctAtk_AtItsAttackSpeed(string id)
        {
            var b = new Battle(Lab.Data, new TeamSetup(id), new TeamSetup("target_dummy"), 5) { DisablePassives = true, ResistFloor = 0 };
            var hero = b.Units[0]; var dummy = b.Units[1];
            hero.Base.CritRate = 0;
            hero.Cd[0] = hero.Cd[1] = 1e9;                 // no skills
            dummy.X = hero.X + 0.8; dummy.Y = hero.Y;       // already in reach
            b.RunFor(10.0);
            var hits = b.Log.Where(e => e.Type == Ev.Damage && e.Src == hero.Index && e.Ab == "basic").ToList();
            Assert.Greater(hits.Count, 0, "no auto attacks");
            foreach (var h in hits) Assert.AreEqual(hero.Atk * b.BasicPctOf(hero), h.Amount, 1e-6, "each auto attack = her auto % x ATK against DEF 0");
            // Ultimate may fire once the bar fills (cast lock pauses attacks), so allow a small shortfall.
            double expected = 10.0 * hero.Base.AtkSpd;
            Assert.LessOrEqual(hits.Count, Math.Ceiling(expected) + 1, "too many attacks for her attack speed");
            Assert.GreaterOrEqual(hits.Count, Math.Floor(expected) - 2, "too few attacks for her attack speed");
        }

        // ---------- abilities vs their text ----------

        public sealed class Claim { public string Kind, Text; public double Value; public override string ToString() => Text; }

        public static List<Claim> ParseClaims(string text)
        {
            var claims = new List<Claim>();
            string rest = text;
            void Take(string pattern, string kind, double scale)
            {
                foreach (Match m in Regex.Matches(rest, pattern))
                    claims.Add(new Claim { Kind = kind, Text = m.Value, Value = double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture) * scale });
                rest = Regex.Replace(rest, pattern, " ");
            }
            Take(@"(\d+(?:\.\d+)?)% ATK", "atk", 0.01);
            Take(@"(\d+(?:\.\d+)?)% (?:of (?:the target's |their |her )?)?max HP(?:/s)?", "maxhp", 0.01);
            Take(@"(\d+(?:\.\d+)?)% HP/s", "maxhp", 0.01);
            Take(@"(\d+(?:\.\d+)?)% of her HP", "maxhp", 0.01);
            Take(@"(\d+(?:\.\d+)?)s\b", "seconds", 1);
            Take(@"(\d+) energy", "energy", 1);
            Take(@"\+(\d+)\)", "flat", 1);                       // "Haste up (+30)"
            Take(@"(\d+(?:\.\d+)?)%", "percent", 0.01);
            return claims;
        }

        /// <summary>Everything one ability did, expressed as numbers the text can be compared with.</summary>
        public sealed class Observed
        {
            public HashSet<double> Atk = new HashSet<double>(), MaxHp = new HashSet<double>(), Seconds = new HashSet<double>(),
                Percent = new HashSet<double>(), Energy = new HashSet<double>(), Flat = new HashSet<double>();
            public List<BattleEvent> Events;
            public static double R(double x) => Math.Round(x, 3);
        }

        public static Observed Run(string id, string key)
        {
            var (b, hero) = Scene(id);
            int from = b.Log.Count;
            b.ForceCast(hero, key);
            hero.CastLock = 1e9;
            b.RunFor(12.0);
            var o = new Observed();
            double hp = 1 + hero.Base.HealPower;
            var mine = b.Log.Skip(from).Where(e => e.Ab == key && e.Src >= 0 &&
                (e.Src == hero.Index || (b.Units[e.Src].IsSummon && b.Units[e.Src].Owner == hero.Index))).ToList();
            o.Events = mine;
            // per-target totals for heal-over-time and damage-over-time
            var healTotals = new Dictionary<int, double>(); var dmgTotals = new Dictionary<int, double>();
            foreach (var e in mine)
            {
                var t = e.Dst >= 0 ? b.Units[e.Dst] : null;
                switch (e.Type)
                {
                    case Ev.Damage:
                        if (e.What == "spend_hp") { o.MaxHp.Add(Observed.R(e.Amount / hero.MaxHp)); break; }
                        if (e.What == "execute") { o.Percent.Add(Observed.R(e.V)); break; }
                        if (e.V > 0) o.Percent.Add(Observed.R(e.V));      // ignored DEF
                        o.Atk.Add(Observed.R(e.Amount / hero.Atk));
                        o.MaxHp.Add(Observed.R(e.Amount / t.MaxHp));
                        dmgTotals[e.Dst] = (dmgTotals.TryGetValue(e.Dst, out var d) ? d : 0) + e.Amount;
                        break;
                    case Ev.Heal:
                        if (e.Amount <= 0) break;
                        if (e.What == "rewind") { o.Seconds.Add(e.V); break; }
                        o.Atk.Add(Observed.R(e.Amount / hero.Atk / hp / (1 + t.Base.HealRecv)));
                        o.MaxHp.Add(Observed.R(e.Amount / t.MaxHp / hp / (1 + t.Base.HealRecv)));
                        if (hero.LastHeal > 0) o.Percent.Add(Observed.R(e.Amount / hp / (1 + t.Base.HealRecv) / (hero.Atk * 2)));
                        healTotals[e.Dst] = (healTotals.TryGetValue(e.Dst, out var h) ? h : 0) + e.Amount;
                        break;
                    case Ev.ShieldGain:
                        {
                            double recv = (1 + hp - 1) * (1 + t.Base.HealRecv);
                            o.Atk.Add(Observed.R(e.Amount / hero.Atk / recv));
                            o.MaxHp.Add(Observed.R(e.Amount / hero.MaxHp / recv));
                            o.MaxHp.Add(Observed.R(e.Amount / t.MaxHp / recv));
                        }
                        break;
                    case Ev.StatusOn:
                        o.Seconds.Add(Observed.R(e.Amount));
                        if (e.What == "regen" || e.What == "poison") o.MaxHp.Add(Observed.R(e.V));
                        else if (e.What == "hot") { o.MaxHp.Add(Observed.R(e.V)); o.Atk.Add(Observed.R(e.V * t.MaxHp / hero.Atk)); }
                        else if (e.What == "burn" || e.What == "bleed" || e.What == "thorns") o.Atk.Add(Observed.R(e.V));
                        else if (e.What == "haste_up" || e.What == "star_mark") { o.Flat.Add(e.V); o.Energy.Add(e.V); }
                        else o.Percent.Add(Observed.R(e.V));
                        if (e.What == "revive_ready" || e.What == "revive_guard") o.Percent.Add(Observed.R(e.V));
                        break;
                    case Ev.Energy: if (e.What != "ult") o.Energy.Add(Math.Abs(Math.Round(e.Amount))); break;
                    case Ev.Zone: o.Seconds.Add(Observed.R(e.Amount)); break;
                    case Ev.Summon: if (e.V > 0) o.Seconds.Add(Observed.R(e.V)); break;
                    case Ev.Revive: o.Percent.Add(Observed.R(e.Amount / t.MaxHp)); break;
                    case Ev.Cooldown: o.Seconds.Add(Observed.R(e.Amount / 2)); break;   // advance applies to both skills
                }
            }
            foreach (var kv in healTotals) { o.MaxHp.Add(Observed.R(kv.Value / b.Units[kv.Key].MaxHp / hp)); o.Atk.Add(Observed.R(kv.Value / hero.Atk / hp)); }
            foreach (var kv in dmgTotals) o.Atk.Add(Observed.R(kv.Value / hero.Atk));
            double dmgAll = dmgTotals.Values.Sum();
            if (dmgAll > 0)     // heals sized as a share of the ability's own damage ("heal for 60% of the damage")
                foreach (var e in mine.Where(e => e.Type == Ev.Heal && e.Amount > 0))
                    o.Percent.Add(Observed.R(e.Amount / hp / (1 + b.Units[e.Dst].Base.HealRecv) / dmgAll));
            return o;
        }

        static bool Matches(Claim c, Observed o)
        {
            bool Near(IEnumerable<double> set, double v) => set.Any(x => Math.Abs(x - v) <= Math.Max(0.006, Math.Abs(v) * 0.02));
            switch (c.Kind)
            {
                case "atk": return Near(o.Atk, c.Value);
                case "maxhp": return Near(o.MaxHp, c.Value) || Near(o.Percent, c.Value);
                case "seconds": return Near(o.Seconds, c.Value) || (c.Value > 2.5 && Near(o.Seconds, 2.5));
                case "energy": return Near(o.Energy, c.Value);
                case "flat": return Near(o.Flat, c.Value);
                case "percent": return Near(o.Percent, c.Value) || Near(o.MaxHp, c.Value) || Near(o.Atk, c.Value);
            }
            return false;
        }

        /// <summary>Hero with three wounded, debuffed ally dummies (one fallen) facing five neutral target dummies in reach.</summary>
        public static (Battle b, Unit hero) Scene(string id)
        {
            var b = new Battle(Lab.Data, new TeamSetup(id, "ally_dummy", "ally_dummy", "ally_dummy", "ally_dummy"),
                               new TeamSetup("target_dummy", "target_dummy", "target_dummy", "target_dummy", "target_dummy"), 11)
            { ResistFloor = 0, DisablePassives = true };
            var hero = b.Units[0];
            hero.Base.CritRate = 0;
            int k = 0;
            foreach (var e in b.Units.Where(u => u.Team == 1)) { e.X = hero.X + 0.9 + 0.3 * (k % 2); e.Y = hero.Y + (k - 2) * 0.35; k++; }
            int j = 0;
            foreach (var a in b.Units.Where(u => u.Team == 0 && u != hero)) { a.X = hero.X - 1.2; a.Y = hero.Y + (j - 1.5) * 0.7; j++; }
            hero.CastLock = 1e9;
            b.RunFor(3.2);
            var allies = b.Units.Where(u => u.Team == 0 && u != hero).ToList();
            foreach (var a in allies) { a.Hp = a.MaxHp * 0.3; b.ApplyStatus(a, a, "slow", 6, 0.3, null); b.ApplyStatus(a, a, "blind", 6, 0.3, null); }
            hero.Hp = hero.MaxHp * 0.8; b.ApplyStatus(hero, hero, "slow", 6, 0.3, null);
            hero.LastHeal = hero.Atk * 2;
            b.Kill(allies[3], null);
            foreach (var e in b.Units.Where(u => u.Team == 1)) { b.ApplyStatus(e, e, "atk_up", 9, 0.1, null); e.Energy = 60; }
            b.Units.Last(u => u.Team == 1).Hp = 100_000 * 0.15;     // a wounded target, for executes
            return (b, hero);
        }

        [TestCaseSource(nameof(Abilities))]
        public void Ability_NumbersMatchItsText(string id, string key)
        {
            var def = Lab.Data.Heroes[id];
            var ab = key == "ult" ? def.Ult : key == "s1" ? def.S1 : def.S2;
            var text = Gacha.Core.Data.Node.Of(Gacha.Core.Data.Json.Parse(HeroesJson)).Nodes("heroes").First(h => h.Str("id") == id)
                .Obj("skills").Obj(key == "ult" ? "ultimate" : key == "s1" ? "skill1" : "skill2").Str("text");
            var o = Run(id, key);
            var missing = ParseClaims(text).Where(c => !Matches(c, o)
                && !(text.Contains("per energy") && c.Kind == "atk" && o.Energy.Any(en => o.Atk.Any(a => Math.Abs(a - c.Value * en) < 0.01)))).ToList();
            CollectionAssert.IsEmpty(missing.Select(c => $"\"{c.Text}\""), $"{id} {key}: \"{text}\"  observed atk[{string.Join(",", o.Atk.Take(8))}] maxhp[{string.Join(",", o.MaxHp.Take(8))}] s[{string.Join(",", o.Seconds)}] %[{string.Join(",", o.Percent)}] energy[{string.Join(",", o.Energy)}]");
        }

        static string _heroesJson;
        static string HeroesJson => _heroesJson ??= System.IO.File.ReadAllText(System.IO.Path.Combine(TestData.Dir, "heroes.json"));

        // ---------- passives vs their text ----------

        [TestCaseSource(nameof(HeroIds))]
        public void Passive_NumbersMatchItsText(string id)
        {
            var node = Gacha.Core.Data.Node.Of(Gacha.Core.Data.Json.Parse(HeroesJson)).Nodes("heroes").First(h => h.Str("id") == id).Obj("skills").Obj("passive");
            var p = Lab.Data.Heroes[id].Passive;
            // every number in the passive's data, in the units the text uses
            var nums = new HashSet<double>();
            foreach (var m in p.Mods)
            {
                nums.Add(Math.Round(Math.Abs(m.V), 3)); nums.Add(Math.Round(Math.Abs(m.V * m.PerBuff), 3));
                if (m.K == "haste") nums.Add(Math.Round(m.V / 100, 3));
            }
            // condition numbers: thresholds (<50) and status windows (stun+3)
            foreach (var cond in p.Mods.Select(m => m.If).Concat(p.Triggers.Select(x => x.If)).Where(x => x != null))
                foreach (Match mm in Regex.Matches(cond, @"[<>+](\d+(?:\.\d+)?)")) nums.Add(double.Parse(mm.Groups[1].Value, CultureInfo.InvariantCulture));
            void Walk(List<Gacha.Core.Data.Node> ops)
            {
                foreach (var n in ops)
                {
                    foreach (var k in new[] { "v", "dur", "pct", "m", "below" }) if (n.Has(k)) nums.Add(Math.Round(Math.Abs(n.Num(k)), 3));
                    Walk(n.Nodes("ops"));
                }
            }
            foreach (var t in p.Triggers) { Walk(t.Ops); if (t.Icd > 0) nums.Add(Math.Round(t.Icd, 3)); }   // "at most once every 2s"
            var missing = new List<string>();
            foreach (var c in ParseClaims(node.Str("text")))
            {
                double v = c.Value;
                bool ok = nums.Any(x => Math.Abs(x - v) < 0.0051) || nums.Contains(c.Kind == "energy" ? v : -1);
                // implicit numbers from the shared rules: "three times" stacks, lifesteal "50% more"
                if (!ok) missing.Add($"\"{c.Text}\"");
            }
            CollectionAssert.IsEmpty(missing, $"{id} passive: \"{node.Str("text")}\" data numbers [{string.Join(",", nums)}]");
        }

        // ---------- passives actually change the numbers they promise ----------

        [TestCaseSource(nameof(HeroIds))]
        public void Passive_DamageBonusesApplyAtTheirValue(string id)
        {
            var def = Lab.Data.Heroes[id];
            var mods = def.Passive.Mods.Where(m => m.K == "dmg_out" && m.Scope == "self" && m.PerStack == null && m.PerBuff == 0).ToList();
            foreach (var m in mods)
            {
                var cond = m.If;
                // build a target that meets the condition where possible
                var b = new Battle(Lab.Data, new TeamSetup(id), new TeamSetup("target_dummy", "target_dummy"), 3) { ResistFloor = 0 };
                var hero = b.Units[0]; var t = b.Units[1];
                if (cond != null)
                {
                    foreach (Match st in Regex.Matches(cond, @"tgt:([a-z_|]+)")) b.ApplyStatus(t, t, st.Groups[1].Value.Split('|')[0], 9, 0.3, null);
                    if (cond.Contains("tgt.grouped")) { b.Units[2].X = t.X + 0.5; b.Units[2].Y = t.Y; }
                    if (cond.Contains("tgt.energy<")) t.Energy = 0;
                    if (cond.Contains("tgt.isolated")) { b.Units[2].X = t.X + 5; }
                    if (cond.Contains("self.beast") || cond.Contains("tgt.summon") || cond.Contains("src:") || cond.Contains("self.hp")) continue;   // covered elsewhere
                }
                double expect = (1 + def.Passive.Mods.Where(x => x.K == "dmg_out" && x.Scope == "self" && x.PerStack == null && x.PerBuff == 0 &&
                                   (x.If == null || b.Eval(x.If, new Cond { Self = hero, Src = hero, Tgt = t }))).Sum(x => x.V));
                double withP = b.Hit(hero, t, 100, new HitInfo { Kind = HitKind.Skill, Ability = "probe" });
                b.DisablePassives = true;
                double without = b.Hit(hero, t, 100, new HitInfo { Kind = HitKind.Skill, Ability = "probe" });
                Assert.Greater(without, 0, $"{id}: probe hit did nothing");
                Assert.AreEqual(expect, withP / without, 1e-6, $"{id}: passive bonus \"{m.If}\" +{m.V}");
                Assert.Greater(withP / without, 1 + m.V - 1e-6, $"{id}: the bonus \"{m.If}\" didn't apply");
            }
        }
    }
}
