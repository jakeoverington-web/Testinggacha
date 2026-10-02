using System;
using System.Collections.Generic;
using System.Linq;
using Gacha.Core;
using Gacha.Core.Battle;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Kinds 3 and 4: nothing impossible ever happens in random battles; a fixed battle replays exactly.</summary>
    [TestFixture]
    public class InvariantTests
    {
        public static List<string> Roster => Lab.Data.HeroOrder.Where(h => !h.StartsWith("dummy") && h != "striker").ToList();

        public static TeamSetup RandomTeam(Rng rng, List<string> pool)
        {
            var picks = new List<string>(pool);
            var team = new TeamSetup();
            for (int i = 0; i < 5; i++) { int k = rng.Range(0, picks.Count); team.Heroes.Add(picks[k]); picks.RemoveAt(k); }
            return team;
        }

        static void CheckTick(Battle b, List<string> problems)
        {
            var T = b.Data.Tuning;
            foreach (var u in b.Units)
            {
                string who = $"t={b.Time:0.0} {u.Id}#{u.Index}";
                if (double.IsNaN(u.Hp) || double.IsNaN(u.X) || double.IsNaN(u.Energy)) problems.Add(who + " NaN");
                if (!u.Alive) continue;
                if (u.Hp <= 0 || u.Hp > u.MaxHp + 1e-6) problems.Add($"{who} hp {u.Hp:0.0}/{u.MaxHp:0.0}");
                if (u.Energy < -1e-9 || u.Energy > 100 + 1e-9) problems.Add($"{who} energy {u.Energy}");
                if (u.ShieldTotal > T.ShieldCap * u.MaxHp + 1e-6) problems.Add($"{who} shield {u.ShieldTotal:0} > cap");
                if (Math.Abs(u.X) > T.HalfWidth + 1e-6 || Math.Abs(u.Y) > T.HalfDepth + 1e-6) problems.Add($"{who} off field");
                int hard = 0;
                foreach (var s in u.Statuses)
                {
                    if (s.Delay > 0) continue;
                    if (b.IsHardControl(s.Id)) { hard++; if (s.Remaining > T.ControlMax + 1e-6) problems.Add($"{who} {s.Id} {s.Remaining:0.00}s > cap"); }
                    if (Battle.Debuffs.Contains(s.Id) && Battle.Buffs.Contains(s.Id)) problems.Add($"{who} {s.Id} is both buff and debuff");
                }
                if (hard > 1) problems.Add($"{who} has {hard} hard controls at once");
            }
        }

        [Test]
        public void RandomBattles_NeverBreakTheRules()
        {
            var rng = new Rng(2026);
            var problems = new List<string>();
            for (int i = 0; i < 80; i++)
            {
                var b = new Battle(Lab.Data, RandomTeam(rng, Roster), RandomTeam(rng, Roster), rng.NextULong()) { KeepLog = false };
                b.AfterTick = x => { if (problems.Count < 20) CheckTick(x, problems); };
                var r = b.Run();
                if (r.Time > b.Data.Tuning.TimeLimit + 1e-6) problems.Add("battle ran past the time limit");
            }
            CollectionAssert.IsEmpty(problems);
        }

        [Test]
        public void AI_UsesEveryAbilityOfEveryHero()
        {
            var rng = new Rng(77);
            var seen = new HashSet<string>();
            for (int i = 0; i < 160; i++)
            {
                var b = new Battle(Lab.Data, RandomTeam(rng, Roster), RandomTeam(rng, Roster), rng.NextULong());
                b.Run();
                foreach (var e in b.Log) if (e.Type == Ev.Cast) seen.Add(b.Units[e.Src].Id + "." + e.What);
            }
            var missing = Roster.SelectMany(h => new[] { "ult", "s1", "s2" }.Select(k => h + "." + k)).Where(x => !seen.Contains(x)).ToList();
            CollectionAssert.IsEmpty(missing, "abilities the AI never cast in 160 random battles");
        }

        [Test]
        public void GoldenReplay_FixedBattleHashIsStable()
        {
            // If a rule change makes this fail on purpose, update the hash in the same commit and say why.
            var a = new TeamSetup("valeria", "ilyra", "aurelle", "seravelle", "lucienne");
            var b = new TeamSetup("sangrael", "nyx", "mordessa", "liora", "ravenna");
            var r = new Battle(Lab.Data, a, b, 2026).Run();
            Assert.AreEqual(GoldenHash, r.Hash, $"winner {r.Winner} at {r.Time:0.0}s; new hash 0x{r.Hash:X16}UL");
        }

        public const ulong GoldenHash = 0xF58209C4B268AEE8UL;   // rules v1 + zone events + dive timing (2026-10-02)
    }
}
