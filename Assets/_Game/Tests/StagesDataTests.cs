using System.Collections.Generic;
using System.Linq;
using Gacha.Core.Campaign;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>The generated Data/stages.json (decisions row 34; built by Tools/StageGen).</summary>
    [TestFixture]
    public class StagesDataTests
    {
        static List<StageDef> Stages => Lab.Data.Stages;

        /// <summary>Heroes whose kit applies or pays off a package (heroes.json "applies" / "payoffs").</summary>
        public static HashSet<string> Carriers(string package)
        {
            var set = new HashSet<string>();
            foreach (var n in Gacha.Core.Data.Node.Of(Gacha.Core.Data.Json.Parse(System.IO.File.ReadAllText(System.IO.Path.Combine(TestData.Dir, "heroes.json")))).Nodes("heroes"))
                if (n.Strs("applies").Contains(package) || n.Strs("payoffs").Contains(package)) set.Add(n.Str("id"));
            return set;
        }

        /// <summary>Weaker early enemies (owner, 2026-10-03): 3 in chapter 1, 4 in chapter 2, then 5.</summary>
        static int TeamSize(int chapter) => chapter == 1 ? 3 : chapter == 2 ? 4 : 5;

        [Test]
        public void EarlyChapters_NormalStagesFollowSmallTemplates()
        {
            var roles = Lab.Data.Heroes;
            foreach (var s in Stages.Where(s => s.Chapter <= 2 && !s.Gate))
            {
                var r = s.Enemies.Select(e => roles[e.Hero].Role).ToArray();
                var want = s.Chapter == 1 ? new[] { "tank", "dps", "healer" } : new[] { "tank", "dps", "dps", "healer" };
                CollectionAssert.AreEqual(want, r, s.Id);
            }
        }

        [Test]
        public void Has600StagesWithStableIds()
        {
            Assert.AreEqual(600, Stages.Count);
            for (int i = 0; i < 600; i++)
            {
                var s = Stages[i];
                int ch = i / 30 + 1, st = i % 30 + 1;
                Assert.AreEqual($"ch{ch:00}_s{st:00}", s.Id);
                Assert.AreEqual(i + 1, s.Index); Assert.AreEqual(ch, s.Chapter); Assert.AreEqual(st, s.Stage);
                Assert.AreEqual(st % 10 == 0, s.Gate, s.Id); Assert.AreEqual(st == 30, s.Boss, s.Id);
            }
        }

        [Test]
        public void EveryEnemyIsARosterHero_LevelsAndStarsInRange()
        {
            var roster = new HashSet<string>(InvariantTests.Roster);
            foreach (var s in Stages)
            {
                Assert.AreEqual(TeamSize(s.Chapter), s.Enemies.Count, s.Id);
                Assert.AreEqual(s.Enemies.Count, s.Enemies.Select(e => e.Hero).Distinct().Count(), s.Id);
                foreach (var e in s.Enemies)
                {
                    Assert.IsTrue(roster.Contains(e.Hero), s.Id + " " + e.Hero);
                    Assert.IsTrue(e.Level >= 1 && e.Level <= 200, s.Id);
                    Assert.AreEqual(Expected.Stars(s.Chapter), e.Stars, s.Id);
                }
                Assert.AreEqual(TeamPower.Of(Lab.Data, s), s.Power, s.Id);
            }
        }

        [Test]
        public void GateStagesShareOnePackage()
        {
            foreach (var s in Stages.Where(s => s.Gate))
            {
                Assert.AreEqual(1, s.Packages.Count, s.Id);
                var carriers = Carriers(s.Packages[0]);
                int inTeam = s.Enemies.Count(e => carriers.Contains(e.Hero));
                int wanted = TeamSize(s.Chapter) - (s.Chapter <= 2 ? 1 : 0);
                Assert.AreEqual(System.Math.Min(wanted, carriers.Count), inTeam, s.Id + " " + s.Packages[0]);
            }
        }

        [Test]
        public void AllTwelvePackagesAppearByChapter6()
        {
            var seen = new HashSet<string>(Stages.Where(s => s.Chapter <= 6).SelectMany(s => s.Packages));
            Assert.AreEqual(12, seen.Count, string.Join(", ", seen));
        }

        [Test]
        public void FirstClearMatchesOneHourOfIdle()
        {
            foreach (var s in Stages)
            {
                Assert.AreEqual(Lab.Data.Idle.PerHour("gold", s.Index), s.FirstClear["gold"], 1, s.Id);
                Assert.AreEqual(Lab.Data.Idle.PerHour("heroXp", s.Index), s.FirstClear["heroXp"], 1, s.Id);
            }
        }
    }
}
