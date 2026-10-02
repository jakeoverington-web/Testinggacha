using System;
using System.Collections.Generic;
using System.Linq;
using Gacha.Core.Battle;
using Gacha.Core.Data;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>
    /// Kind 2: generated from heroes.json. Every ability of every hero is force-cast in a prepared scene and must leave
    /// the evidence its tags promise (a "cc_stun" skill must stun, a "heal_aoe" skill must heal...). No per-hero code:
    /// a new hero gets these tests automatically.
    /// </summary>
    [TestFixture]
    public class KitTests
    {
        public static IEnumerable<string> HeroIds => Lab.Data.HeroOrder;
        public static IEnumerable<object[]> Abilities =>
            Lab.Data.HeroOrder.SelectMany(h => new[] { "ult", "s1", "s2" }.Select(k => new object[] { h, k }));

        static AbilityDef Ab(HeroDef d, string k) => k == "ult" ? d.Ult : k == "s1" ? d.S1 : d.S2;

        // ---------- data validation ----------

        [TestCaseSource(nameof(HeroIds))]
        public void Data_UsesOnlyKnownVocabulary(string id)
        {
            var d = Lab.Data.Heroes[id];
            var problems = new List<string>();
            foreach (var k in new[] { "ult", "s1", "s2" })
            {
                var ab = Ab(d, k);
                if (ab.Ops.Count == 0) problems.Add(k + ": no ops");
                CheckOps(ab.Ops, k, problems);
                if (k != "ult" && ab.Cooldown <= 0) problems.Add(k + ": no cooldown");
            }
            foreach (var m in d.Passive.Mods)
            {
                if (!Battle.ModKinds.Contains(m.K)) problems.Add("passive mod kind " + m.K);
                if (m.If != null && Battle.CheckCondition(m.If) is string e1) problems.Add("passive mod if: " + e1);
            }
            foreach (var t in d.Passive.Triggers)
            {
                if (!Battle.TriggerEvents.Contains(t.On)) problems.Add("trigger event " + t.On);
                if (t.If != null && Battle.CheckCondition(t.If) is string e2) problems.Add("trigger if: " + e2);
                CheckOps(t.Ops, "trigger " + t.On, problems);
            }
            foreach (var f in d.Passive.Flags)
                if (!new[] { "prefer_marked", "no_revive_on_kill", "see_stealth", "immune:burn" }.Contains(f)) problems.Add("flag " + f);
            CollectionAssert.IsEmpty(problems, id);
        }

        static void CheckOps(List<Node> ops, string where, List<string> problems)
        {
            foreach (var n in ops)
            {
                string op = n.Str("op");
                if (!Battle.OpNames.Contains(op)) { problems.Add($"{where}: op {op}"); continue; }
                string to = n.Str("to");
                if (to != null)
                {
                    int c = to.IndexOf(':'); string name = c < 0 ? to : to.Substring(0, c);
                    if (!Battle.Selectors.Contains(name)) problems.Add($"{where}: selector {to}");
                }
                if (op == "status" && !Battle.Buffs.Contains(n.Str("s")) && !Battle.Debuffs.Contains(n.Str("s"))) problems.Add($"{where}: status {n.Str("s")}");
                if (op == "summon" && !Lab.Data.Tuning.Summons.ContainsKey(n.Str("unit"))) problems.Add($"{where}: summon {n.Str("unit")}");
                CheckOps(n.Nodes("ops"), where + "/zone", problems);
                CheckOps(n.Nodes("then"), where + "/then", problems);
            }
        }

        [Test]
        public void Data_Has60HeroesAndEveryRaceHas12()
        {
            var real = Lab.Data.HeroOrder.Where(h => !h.StartsWith("dummy") && h != "striker").ToList();
            Assert.AreEqual(60, real.Count);
            foreach (var core in new[] { "high", "dark", "nature", "ocean", "arcane" })
                Assert.AreEqual(12, real.Count(h => Lab.Data.Heroes[h].Core == core), core);
        }

        // ---------- behaviour: every ability leaves the evidence its tags promise ----------

        static readonly Dictionary<string, Func<BattleEvent, Battle, bool>> Evidence = new Dictionary<string, Func<BattleEvent, Battle, bool>>
        {
            ["dmg_single"] = (e, b) => e.Type == Ev.Damage, ["dmg_aoe"] = (e, b) => e.Type == Ev.Damage, ["dmg_row"] = (e, b) => e.Type == Ev.Damage,
            ["execute"] = (e, b) => e.Type == Ev.Damage,
            ["heal_single"] = (e, b) => (e.Type == Ev.Heal && e.Amount > 0) || On(e, "hot", "regen"),
            ["heal_aoe"] = (e, b) => (e.Type == Ev.Heal && e.Amount > 0) || On(e, "hot", "regen"),
            ["shield_single"] = (e, b) => e.Type == Ev.ShieldGain || On(e, "skill_shield", "hit_shield"),
            ["shield_aoe"] = (e, b) => e.Type == Ev.ShieldGain || On(e, "skill_shield", "hit_shield"),
            ["revive"] = (e, b) => e.Type == Ev.Revive || On(e, "revive_ready", "revive_guard"),
            ["regen"] = (e, b) => On(e, "regen", "hot") || (e.Type == Ev.Heal && e.Amount > 0),
            ["taunt"] = (e, b) => On(e, "taunt") || e.Type == Ev.Summon,
            ["cc_stun"] = (e, b) => On(e, "stun", "stasis"), ["cc_root"] = (e, b) => On(e, "root"), ["cc_sleep"] = (e, b) => On(e, "sleep"),
            ["cc_fear"] = (e, b) => On(e, "fear"), ["cc_charm"] = (e, b) => On(e, "charm"), ["cc_silence"] = (e, b) => On(e, "silence"),
            ["cc_knock"] = (e, b) => On(e, "airborne") || e.Type == Ev.Move, ["pull"] = (e, b) => e.Type == Ev.Move,
            ["debuff_slow"] = (e, b) => On(e, "slow"), ["debuff_blind"] = (e, b) => On(e, "blind"), ["debuff_def"] = (e, b) => On(e, "def_down"),
            ["debuff_weaken"] = (e, b) => On(e, "weaken", "mark"), ["debuff_antiheal"] = (e, b) => On(e, "antiheal"), ["debuff_atk"] = (e, b) => On(e, "dmg_down"),
            ["mark"] = (e, b) => On(e, "mark"), ["curse"] = (e, b) => On(e, "curse", "feeding_curse"),
            ["dot_burn"] = (e, b) => On(e, "burn", "dragonform"), ["dot_bleed"] = (e, b) => On(e, "bleed"), ["dot_poison"] = (e, b) => On(e, "poison"),
            ["soak"] = (e, b) => On(e, "soaked"),
            ["buff_atk"] = (e, b) => On(e, "atk_up"), ["buff_def"] = (e, b) => On(e, "def_up", "dragonform"), ["buff_speed"] = (e, b) => On(e, "aspd_up"),
            ["buff_crit"] = (e, b) => On(e, "crit_up", "sure_crit"), ["buff_lifesteal"] = (e, b) => On(e, "lifesteal_up"),
            ["buff_cdr"] = (e, b) => e.Type == Ev.Cooldown || On(e, "haste_up"),
            ["buff_energy"] = (e, b) => e.Type == Ev.Energy && e.Amount > 0 && e.Dst != e.Src,
            ["energy_drain"] = (e, b) => e.Type == Ev.Energy && e.Amount < 0 && b.Units[e.Dst].Team != b.Units[Math.Max(0, e.Src)].Team,
            ["cleanse"] = (e, b) => e.Type == Ev.Cleanse, ["dispel"] = (e, b) => e.Type == Ev.Dispel,
            ["untargetable"] = (e, b) => On(e, "untargetable", "stealth") || e.Type == Ev.Move,
            ["immunity"] = (e, b) => On(e, "cc_immunity", "immunity"), ["unkillable"] = (e, b) => On(e, "unkillable"),
            ["dr"] = (e, b) => On(e, "redirect", "dr", "reflect", "share", "projectile_block"),
            ["reflect"] = (e, b) => On(e, "reflect", "thorns", "thread"), ["counter"] = (e, b) => On(e, "counter"),
            ["summon"] = (e, b) => e.Type == Ev.Summon,
        };

        static bool On(BattleEvent e, params string[] ids) => e.Type == Ev.StatusOn && ids.Contains(e.What);

        /// <summary>Hero plus three wounded, debuffed allies and one fallen ally, facing five buffed training targets in reach.</summary>
        public static (Battle b, Unit hero) Scene(string id)
        {
            var b = new Battle(Lab.Data, new TeamSetup(id, "dummy", "dummy", "dummy", "dummy"),
                               new TeamSetup("dummy", "dummy", "dummy", "dummy", "dummy"), 11);
            b.ResistFloor = 0;
            var hero = b.Units[0];
            foreach (var u in b.Units)
            {
                if (u.Team == 0 && u != hero) { u.Base.Hp = 10000; u.Hp = 10000; u.Base.MoveSpd = 0; }
                if (u.Team == 1) { u.Base.Hp = 100000; u.Hp = 100000; u.Base.Def = 100; }
            }
            // put the enemies within melee reach of the hero, grouped
            double dir = 1;
            int k = 0;
            foreach (var e in b.Units.Where(u => u.Team == 1)) { e.X = hero.X + dir * (0.9 + 0.3 * (k % 2)); e.Y = hero.Y + (k - 2) * 0.35; k++; }
            int j = 0;
            foreach (var a in b.Units.Where(u => u.Team == 0 && u != hero)) { a.X = hero.X - 1.2; a.Y = hero.Y + (j - 1.5) * 0.7; j++; }
            hero.CastLock = 1e9;
            b.RunFor(3.2);                       // fill the 3-second HP history (for rewinds)
            var allies = b.Units.Where(u => u.Team == 0 && u != hero).ToList();
            foreach (var a in allies.Append(hero)) { a.Hp = a.MaxHp * 0.5; b.ApplyStatus(a, a, "slow", 6, 0.3, null); b.ApplyStatus(a, a, "blind", 6, 0.3, null); }
            hero.Hp = hero.MaxHp * 0.8;          // allies are hurt worse, so ally-targeted skills pick them
            hero.LastHeal = hero.Atk * 2;        // as if she had healed before (Siora's Echo)
            b.Kill(allies[3], null);
            foreach (var e in b.Units.Where(u => u.Team == 1)) { b.ApplyStatus(e, e, "atk_up", 9, 0.1, null); b.ApplyStatus(e, e, "def_up", 9, 0.1, null); e.Energy = 60; }
            hero.Energy = 100;
            return (b, hero);
        }

        [TestCaseSource(nameof(Abilities))]
        public void Ability_DoesWhatItsTagsSay(string id, string key)
        {
            var (b, hero) = Scene(id);
            int from = b.Log.Count;
            b.ForceCast(hero, key);
            hero.CastLock = 1e9;
            // pressure: both sides trade blows so on-hit and on-damage effects can show
            var foes = b.Units.Where(u => u.Team == 1 && u.IsHero).ToList();
            for (int i = 0; i < 7; i++)
            {
                foreach (var a in b.Units.Where(u => u.Team == 0 && u.Alive && u.IsHero).ToList())
                {
                    b.Hit(foes[0], a, a.MaxHp * 0.04, new HitInfo { Kind = HitKind.Basic, Ability = "pressure" });
                    if (a != hero) b.Hit(a, foes[i % foes.Count], 10, new HitInfo { Kind = HitKind.Basic, Ability = "pressure" });
                }
                b.RunFor(0.5);
            }
            var mine = b.Log.Skip(from).Where(e => e.Ab == key && e.Src >= 0 &&
                (e.Src == hero.Index || (b.Units[e.Src].IsSummon && b.Units[e.Src].Owner == hero.Index))).ToList();
            var missing = new List<string>();
            foreach (var tag in Ab(Lab.Data.Heroes[id], key).Tags)
            {
                if (!Evidence.TryGetValue(tag, out var test)) { missing.Add(tag + " (no evidence rule)"); continue; }
                if (!mine.Any(e => test(e, b))) missing.Add(tag);
            }
            CollectionAssert.IsEmpty(missing, $"{id} {key} events: " + string.Join(" | ", mine.Take(12)));
        }
    }
}
