using System;
using Gacha.Core.Economy;
using Gacha.Core.Progression;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Owned heroes, Contract slots, levels and sync (decisions rows 7, 31; phase 2 spec).</summary>
    [TestFixture]
    public class CollectionTests
    {
        static Gacha.Core.Battle.GameData G => Lab.Data;

        static Collection WithFive(out string[] ids)
        {
            ids = new[] { "cassia", "nyx", "aurelle", "halcyra", "isolde" };
            var c = new Collection();
            for (int i = 0; i < ids.Length; i++) { c.Add(ids[i]); c.Contract(i, ids[i]); }
            return c;
        }

        static Wallet Rich() { var w = new Wallet(); w.Add("heroXp", 1e12); w.Add("gold", 1e12); w.Add("starlight", 1e9); return w; }

        [Test]
        public void Add_FirstIsOneStar_LaterAreCopies()
        {
            var c = new Collection();
            c.Add("cassia"); c.Add("cassia"); c.Add("cassia");
            Assert.IsTrue(c.Owns("cassia"));
            Assert.AreEqual(1, c.Get("cassia").Stars);
            Assert.AreEqual(2, c.Get("cassia").Copies);
        }

        [Test]
        public void LevelUp_SpendsTheLevelsCost()
        {
            var c = WithFive(out _); var w = Rich();
            double xp = w.Get("heroXp"), gold = w.Get("gold");
            for (int l = 2; l <= 20; l++) Assert.IsTrue(c.LevelUp(G, 0, w));
            Assert.AreEqual(20, c.Slots[0].Level);
            double spentXp = 0, spentGold = 0; for (int l = 2; l <= 20; l++) { spentXp += G.Levels.Xp(l); spentGold += G.Levels.Gold(l); }
            Assert.AreEqual(xp - spentXp, w.Get("heroXp"), 1e-6);
            Assert.AreEqual(gold - spentGold, w.Get("gold"), 1e-6);
            Assert.AreEqual(1e9 - 10, w.Get("starlight"), 1e-6, "Starlight at level 20");
        }

        [Test]
        public void LevelUp_RefusedAtStarCap_AndWhenShort()
        {
            var c = WithFive(out _); var w = Rich();
            c.Slots[0].Level = 60;                               // 1-star cap
            Assert.IsFalse(c.LevelUp(G, 0, w));
            var poor = new Wallet();
            Assert.IsFalse(c.LevelUp(G, 1, poor));
            Assert.AreEqual(1, c.Slots[1].Level);
            c.Slots[2].Hero = "";
            Assert.IsFalse(c.LevelUp(G, 2, w), "an empty slot cannot level");
        }

        [Test]
        public void HighSlot_ShowsTheHerosCap_SlotKeepsItsLevel()
        {
            var c = WithFive(out var ids);
            c.Slots[0].Level = 100;
            Assert.AreEqual(60, c.Level(G, ids[0]), "a 1-star hero shows her cap");
            Assert.AreEqual(100, c.Slots[0].Level, "the slot keeps its level");
            c.Add("valeria"); c.Get("valeria").Stars = 5;
            c.Contract(0, "valeria");
            Assert.AreEqual(100, c.Level(G, "valeria"), "a 5-star hero in the same slot gets the full level");
        }

        [Test]
        public void Everyone_Else_SyncsToTheLowestSlot()
        {
            var c = WithFive(out _);
            int[] levels = { 50, 60, 70, 80, 90 };
            for (int i = 0; i < 5; i++) c.Slots[i].Level = levels[i];
            c.Add("sable"); c.Get("sable").Stars = 1;
            Assert.AreEqual(50, c.Level(G, "sable"));
            Assert.AreEqual(50, c.SyncLevel);
        }

        [Test]
        public void Contract_MovesAHero_AndRequiresOwnership()
        {
            var c = WithFive(out var ids);
            c.Contract(4, ids[0]);
            Assert.AreEqual(ids[0], c.Slots[4].Hero);
            Assert.AreEqual("", c.Slots[0].Hero, "a hero sits in one slot at a time");
            Assert.Throws<InvalidOperationException>(() => c.Contract(0, "ravenna"));
            Assert.IsTrue(c.InSlot(ids[0])); Assert.IsTrue(c.InSlot(ids[1]));
            Assert.IsFalse(c.InSlot("sable"));
        }

        [Test]
        public void ResetSlot_RefundsEverything()
        {
            var c = WithFive(out _); var w = Rich();
            var before = (w.Get("heroXp"), w.Get("gold"), w.Get("starlight"));
            for (int l = 2; l <= 45; l++) c.LevelUp(G, 0, w);
            c.ResetSlot(G, 0, w);
            Assert.AreEqual(1, c.Slots[0].Level);
            Assert.AreEqual(before.Item1, w.Get("heroXp"), 1e-3);
            Assert.AreEqual(before.Item2, w.Get("gold"), 1e-3);
            Assert.AreEqual(before.Item3, w.Get("starlight"), 1e-3);
        }
    }
}
