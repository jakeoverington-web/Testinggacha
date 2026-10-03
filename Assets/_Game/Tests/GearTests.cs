using System.Linq;
using Gacha.Core;
using Gacha.Core.Economy;
using Gacha.Core.Gear;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Gear: 4 slots, 13 types, 4 rarities, 5 sets, upgrades, random drops (decisions row 6; phase 2 spec).</summary>
    [TestFixture]
    public class GearTests
    {
        static GearCatalog C => Lab.Data.Gear;

        [Test]
        public void MainStat_ByRarityAndUpgrade_ExtraByRarity()
        {
            var inv = new Inventory();
            var w = inv.Add("keen", "fury", 4);                  // Legendary weapon
            inv.Equip(C, w, "nyx");
            var b = inv.BonusFor(C, "nyx");
            Assert.AreEqual(0.40, b.AtkPct, 1e-9);
            Assert.AreEqual(0.15, b.CritRate, 1e-9);
            w.Upgrade = 20;
            Assert.AreEqual(0.80, inv.BonusFor(C, "nyx").AtkPct, 1e-9, "+20 doubles the main stat");
            var boots = inv.Add("charged", "vigor", 1);          // Uncommon boots
            inv.Equip(C, boots, "nyx");
            b = inv.BonusFor(C, "nyx");
            Assert.AreEqual(10, b.Haste, 1e-9);
            Assert.AreEqual(0.05, b.EnergyRegen, 1e-9, "Uncommon = 1/4 of the Legendary 20%");
        }

        [Test]
        public void SetBonuses_TwoAndFourPiece()
        {
            var inv = new Inventory();
            inv.Equip(C, inv.Add("keen", "vigor", 1), "nyx");
            Assert.AreEqual(0, inv.BonusFor(C, "nyx").StartEnergy);
            inv.Equip(C, inv.Add("warded", "vigor", 1), "nyx");
            var two = inv.BonusFor(C, "nyx");
            Assert.AreEqual(0.10, two.EnergyRegen, 1e-9, "2-piece");
            inv.Equip(C, inv.Add("guarding", "vigor", 1), "nyx");
            inv.Equip(C, inv.Add("quick", "vigor", 1), "nyx");
            var four = inv.BonusFor(C, "nyx");
            Assert.AreEqual(30, four.StartEnergy, 1e-9, "4-piece");
            Assert.AreEqual(0.10, four.EnergyRegen, 1e-9, "2-piece still counts");
        }

        [Test]
        public void OneOwnerPerItem_AndOnePerSlot()
        {
            var inv = new Inventory();
            var a = inv.Add("keen", "fury", 2); var b = inv.Add("brutal", "fury", 3);
            inv.Equip(C, a, "nyx");
            inv.Equip(C, a, "cassia");
            Assert.AreEqual("cassia", a.EquippedOn);
            Assert.AreEqual(0, inv.On("nyx").Count());
            inv.Equip(C, b, "cassia");
            Assert.AreEqual("", a.EquippedOn, "the old weapon comes off");
            Assert.AreEqual(1, inv.On("cassia").Count());
            Assert.IsTrue(inv.IsGeared("cassia")); Assert.IsFalse(inv.IsGeared("nyx"));
        }

        [Test]
        public void Upgrade_CostsGold_StopsAt20_SalvageReturnsIt()
        {
            var inv = new Inventory(); var item = inv.Add("keen", "fury", 2);
            var w = new Wallet(); w.Add("gold", 1e9);
            Assert.IsTrue(inv.Upgrade(C, item, w));
            Assert.AreEqual(1e9 - 1000 * 2 * 1, w.Get("gold"), 1e-6);
            item.Upgrade = 20;
            Assert.IsFalse(inv.Upgrade(C, item, w));
            var poor = new Wallet(); item.Upgrade = 0;
            Assert.IsFalse(inv.Upgrade(C, item, poor));
            item.Upgrade = 3;
            long value = inv.SalvageValue(C, item);
            Assert.AreEqual(2 * 1000 + 1000 * 2 * (1 + 2 + 3), value, "base + all upgrade gold");
            inv.Salvage(C, item, w);
            Assert.IsFalse(inv.Items.Contains(item));
        }

        [Test]
        public void EquipBest_PicksTheStrongestFreePiecePerSlot()
        {
            var inv = new Inventory();
            inv.Add("keen", "fury", 1); var best = inv.Add("brutal", "fury", 3); var taken = inv.Add("hexing", "fury", 4);
            inv.Equip(C, taken, "cassia");
            var helm = inv.Add("warded", "bulwark", 2);
            inv.EquipBest(C, "nyx");
            Assert.AreEqual("nyx", best.EquippedOn, "best free weapon");
            Assert.AreEqual("cassia", taken.EquippedOn, "never strips another hero");
            Assert.AreEqual("nyx", helm.EquippedOn);
        }

        [Test]
        public void Rarities_UnlockAfterChapterBosses()
        {
            Assert.AreEqual(1, GearDrops.UnlockedRarities(C, 0));
            Assert.AreEqual(1, GearDrops.UnlockedRarities(C, 149));
            Assert.AreEqual(2, GearDrops.UnlockedRarities(C, 150));
            Assert.AreEqual(3, GearDrops.UnlockedRarities(C, 300));
            Assert.AreEqual(4, GearDrops.UnlockedRarities(C, 450));
        }

        [Test]
        public void DropOdds_FavourTheNewest_Renormalised()
        {
            var rng = new Rng(11); var inv = new Inventory();
            int[] counts = new int[5];
            for (int i = 0; i < 20000; i++) counts[GearDrops.Roll(rng, C, 2, inv).Rarity]++;
            Assert.AreEqual(0.625, counts[2] / 20000.0, 0.015);
            Assert.AreEqual(0.375, counts[1] / 20000.0, 0.015);
            Assert.AreEqual(0, counts[3] + counts[4]);
            var all = Enumerable.Range(0, 20000).Select(_ => GearDrops.Roll(rng, C, 4, inv)).ToList();
            Assert.AreEqual(0.50, all.Count(x => x.Rarity == 4) / 20000.0, 0.015);
            Assert.AreEqual(0.05, all.Count(x => x.Rarity == 1) / 20000.0, 0.01);
            Assert.AreEqual(13, all.Select(x => x.Type).Distinct().Count());
            Assert.AreEqual(5, all.Select(x => x.Set).Distinct().Count());
        }

        [Test]
        public void IdleGear_MostlyOneOrTwo_ScalesWithHours()
        {
            var rng = new Rng(4);
            var full = Enumerable.Range(0, 20000).Select(_ => GearDrops.IdleCount(rng, C, 24)).ToList();
            Assert.AreEqual(0.50, full.Count(n => n == 1) / 20000.0, 0.015);
            Assert.AreEqual(0.03, full.Count(n => n == 4) / 20000.0, 0.006);
            Assert.AreEqual(0, full.Count(n => n == 0 || n > 4));
            double avgHalf = Enumerable.Range(0, 20000).Select(_ => GearDrops.IdleCount(rng, C, 12)).Average();
            Assert.AreEqual(1.68 / 2, avgHalf, 0.05);
            Assert.AreEqual(0, GearDrops.IdleCount(rng, C, 0));
        }
    }
}
