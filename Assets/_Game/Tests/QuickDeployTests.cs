using System.Collections.Generic;
using System.Linq;
using Gacha.Core.Campaign;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Quick Deploy: top 5 by power, tanks in the front slots (owner, 2026-10-03).</summary>
    [TestFixture]
    public class QuickDeployTests
    {
        static double P(string h) => TeamPower.Hero(Lab.Data, h, 10, 1);
        static List<string> Roster => InvariantTests.Roster;
        static bool Tank(string h) => !string.IsNullOrEmpty(h) && Lab.Data.Heroes[h].Role == "tank";

        [Test]
        public void PicksTop5ByPower()
        {
            var slots = QuickDeploy.Pick(Lab.Data, Roster, 10, 1);
            var top5 = Roster.OrderByDescending(P).Take(5).ToList();
            Assert.AreEqual(5, slots.Count);
            CollectionAssert.AreEquivalent(top5, slots);
        }

        [Test]
        public void TanksTakeFrontSlots()
        {
            var tanks = Roster.Where(Tank).OrderBy(P).Take(2).ToList();           // the two weakest tanks
            var others = Roster.Where(h => !Tank(h)).OrderByDescending(P).Take(3).ToList();
            var slots = QuickDeploy.Pick(Lab.Data, tanks.Concat(others), 10, 1);
            Assert.IsTrue(Tank(slots[0]) && Tank(slots[1]), string.Join(",", slots));
            CollectionAssert.AreEqual(others, slots.Skip(2).ToList(), "back row in power order");
        }

        [Test]
        public void FewTanks_FrontFilledByStrongest()
        {
            var tank = Roster.First(Tank);
            var others = Roster.Where(h => !Tank(h)).OrderByDescending(P).Take(4).ToList();
            var slots = QuickDeploy.Pick(Lab.Data, new[] { tank }.Concat(others), 10, 1);
            Assert.AreEqual(tank, slots[0]);
            Assert.AreEqual(others[0], slots[1]);
        }

        [Test]
        public void TiesBreakByGivenOrder()
        {
            // dummy and dummy_dark have identical stats, so identical power.
            CollectionAssert.AreEqual(new[] { "dummy_dark", "dummy", "", "", "" }, QuickDeploy.Pick(Lab.Data, new[] { "dummy_dark", "dummy" }, 10, 1));
            CollectionAssert.AreEqual(new[] { "dummy", "dummy_dark", "", "", "" }, QuickDeploy.Pick(Lab.Data, new[] { "dummy", "dummy_dark" }, 10, 1));
        }

        [Test]
        public void FewerThan5Owned_LeavesEmptySlots()
        {
            var slots = QuickDeploy.Pick(Lab.Data, Roster.Where(h => !Tank(h)).Take(3), 10, 1);
            Assert.AreEqual(5, slots.Count);
            Assert.AreEqual(3, slots.Count(h => h != ""));
            CollectionAssert.AreEqual(new[] { "", "" }, slots.Skip(3).ToList());
        }
    }
}
