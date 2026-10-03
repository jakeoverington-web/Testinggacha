using System.IO;
using System.Linq;
using Gacha.Core.Data;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Phase 2 data: level costs, star costs, gear catalogue, maxStars (phase 2 spec; plan doc tables).</summary>
    [TestFixture]
    public class ProgressionDataTests
    {
        static Gacha.Core.Battle.GameData G => Lab.Data;

        [Test]
        public void LevelCaps_PerStar()
        {
            CollectionAssert.AreEqual(new[] { 60, 70, 80, 90, 100, 120, 140, 160, 180, 200 }, Enumerable.Range(1, 10).Select(G.Levels.Cap).ToArray());
        }

        [Test]
        public void LevelCosts_MatchPlanTableAtBreakpoints()
        {
            Assert.AreEqual(137, G.Levels.Xp(20), 0.5); Assert.AreEqual(150, G.Levels.Gold(20), 0.5);
            Assert.AreEqual(13440, G.Levels.Xp(100), 0.5); Assert.AreEqual(14000, G.Levels.Gold(100), 0.5);
            Assert.AreEqual(700000, G.Levels.Xp(200), 0.5); Assert.AreEqual(740000, G.Levels.Gold(200), 0.5);
            Assert.AreEqual(0, G.Levels.Xp(1));
        }

        [Test]
        public void LevelCosts_CumulativeWithin6PercentOfPlan()
        {
            var plan = new (int level, double xp, double gold)[] { (20, 1300, 1900), (60, 20000, 29000), (100, 190000, 290000), (160, 2.9e6, 4.4e6), (200, 14e6, 21e6) };
            foreach (var (level, xp, gold) in plan)
            {
                double cx = 0, cg = 0;
                for (int l = 2; l <= level; l++) { cx += G.Levels.Xp(l); cg += G.Levels.Gold(l); }
                Assert.AreEqual(xp * 1.4, cx, xp * 1.4 * 0.06, "XP to " + level + " (plan x1.4, economy gate)");
                Assert.AreEqual(gold, cg, gold * 0.06, "gold to " + level);
            }
        }

        [Test]
        public void Starlight_AtEvery20thLevel()
        {
            Assert.AreEqual(10, G.Levels.Starlight(20)); Assert.AreEqual(11000, G.Levels.Starlight(180));
            Assert.AreEqual(0, G.Levels.Starlight(21)); Assert.AreEqual(0, G.Levels.Starlight(200));
            Assert.AreEqual(22450, Enumerable.Range(1, 200).Sum(G.Levels.Starlight));
        }

        [Test]
        public void StarCosts_TotalEightCopies15Fodder41MGold()
        {
            var rows = Enumerable.Range(2, 9).Select(G.StarCosts.For).ToList();
            Assert.AreEqual(8, rows.Sum(r => r.Copies));
            Assert.AreEqual(15, rows.Sum(r => r.Fodder));
            Assert.AreEqual(4_100_000, rows.Sum(r => r.Gold));
            Assert.AreEqual(1, G.StarCosts.For(2).Copies); Assert.AreEqual(0, G.StarCosts.For(2).Fodder);
            Assert.AreEqual(4, G.StarCosts.For(10).Fodder);
        }

        [Test]
        public void GearCatalog_Shape()
        {
            var c = G.Gear;
            CollectionAssert.AreEqual(new[] { "weapon", "helm", "armor", "boots" }, c.Slots.Select(s => s.Id).ToArray());
            Assert.AreEqual(13, c.Types.Count);
            Assert.AreEqual(4, c.Types.Count(t => t.Slot == "weapon"));
            CollectionAssert.AreEqual(new[] { "uncommon", "rare", "epic", "legendary" }, c.Rarities.Select(r => r.Id).ToArray());
            CollectionAssert.AreEqual(new[] { 0.10, 0.20, 0.30, 0.40 }, c.Rarities.Select(r => r.MainPct).ToArray());
            CollectionAssert.AreEqual(new[] { 0, 5, 10, 15 }, c.Rarities.Select(r => r.UnlockAfterChapter).ToArray());
            Assert.AreEqual(5, c.Sets.Count);
            Assert.AreEqual(0.15, c.Types.First(t => t.Id == "keen").Value, 1e-9);
            Assert.AreEqual(20, c.MaxUpgrade); Assert.AreEqual(1000, c.UpgradeGoldPerRankLevel);
        }

        [Test]
        public void Heroes_MaxStarsIsTen()
        {
            var heroes = Node.Of(Json.Parse(File.ReadAllText(Path.Combine(TestData.Dir, "heroes.json")))).Nodes("heroes");
            Assert.AreEqual(60, heroes.Count);
            Assert.IsTrue(heroes.All(h => h.Num("maxStars") == 10));
        }
    }
}
