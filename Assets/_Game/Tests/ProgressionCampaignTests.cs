using System;
using System.Collections.Generic;
using System.Linq;
using Gacha.Core.Battle;
using Gacha.Core.Campaign;
using Gacha.Core.Progression;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Phase 2 in the campaign: starter team, real levels/stars/gear/ranks, gear drops, save v2 (phase 2 spec).</summary>
    [TestFixture]
    public class ProgressionCampaignTests
    {
        const long T0 = 1_800_000_000;

        // Stage 1 a weakling (always won), stage 2 a weakling chapter boss (always won: drops gear).
        const string StagesJson = @"{ ""stages"": [
            { ""id"": ""ch01_s01"", ""index"": 1, ""chapter"": 1, ""stage"": 1, ""enemies"": [ { ""hero"": ""weakling"", ""level"": 1, ""stars"": 1 } ], ""firstClear"": { ""gold"": 500 } },
            { ""id"": ""ch01_s02"", ""index"": 2, ""chapter"": 1, ""stage"": 2, ""boss"": true, ""gate"": true, ""enemies"": [ { ""hero"": ""weakling"", ""level"": 1, ""stars"": 1 } ], ""firstClear"": { ""gold"": 600 } } ] }";

        static GameData _g;
        static GameData G
        {
            get
            {
                if (_g != null) return _g;
                _g = GameData.Load(TestData.Dir);
                _g.Heroes["weakling"] = TestData.Dummy("weakling", "none", hp: 10, atk: 0, def: 0, atkSpd: 0);
                _g.Stages = StageDef.ListFromJson(StagesJson);
                return _g;
            }
        }

        [Test]
        public void Starter_FiveUnique_AtLeastOneTankAndHealer_Seeded()
        {
            for (ulong seed = 1; seed <= 200; seed++)
            {
                var team = Starter.Roll(G, seed);
                Assert.AreEqual(5, team.Distinct().Count());
                Assert.IsTrue(team.Any(h => G.Heroes[h].Role == "tank"), "seed " + seed);
                Assert.IsTrue(team.Any(h => G.Heroes[h].Role == "healer"), "seed " + seed);
                CollectionAssert.AreEqual(team, Starter.Roll(G, seed), "same seed, same team");
            }
            Assert.AreNotEqual(string.Join(",", Starter.Roll(G, 1)), string.Join(",", Starter.Roll(G, 2)));
        }

        [Test]
        public void NewGame_OwnsTheStarterTeam_InTheSlots()
        {
            var st = CampaignState.NewGame(G, 7, T0);
            var team = Starter.Roll(G, 7);
            Assert.AreEqual(5, st.Collection.Heroes.Count);
            for (int i = 0; i < 5; i++) { Assert.AreEqual(team[i], st.Collection.Slots[i].Hero); Assert.AreEqual(1, st.Collection.Slots[i].Level); }
            Assert.IsFalse(st.UseExpectedLevels);
        }

        [Test]
        public void PlayerSetup_UsesRealLevelStarsGearAndRanks()
        {
            var st = CampaignState.NewGame(G, 7, T0);
            string h = st.Collection.Slots[0].Hero;
            st.Collection.Slots[0].Level = 30;
            st.Collection.Get(h).Stars = 3;
            var item = st.Inventory.Add("keen", "fury", 2);
            st.Inventory.Equip(G.Gear, item, h);
            var setup = st.PlayerSetup(G, 1, new[] { h });
            Assert.AreEqual(G.Progression.Mult(30, 3), setup.Scales[0], 1e-9);
            Assert.AreEqual(0.20, setup.Bonuses[0].AtkPct, 1e-9);
            CollectionAssert.AreEqual(new[] { 1, 2, 2, 1 }, setup.Ranks[0], "3 stars: skill 1 and skill 2 at rank 2");
        }

        [Test]
        public void Ranks_FollowTheStarTable()
        {
            CollectionAssert.AreEqual(new[] { 1, 1, 1, 1 }, CampaignState.RanksFor(1));
            CollectionAssert.AreEqual(new[] { 2, 2, 2, 2 }, CampaignState.RanksFor(5));
            CollectionAssert.AreEqual(new[] { 2, 3, 3, 2 }, CampaignState.RanksFor(7));
            CollectionAssert.AreEqual(new[] { 3, 3, 3, 3 }, CampaignState.RanksFor(10));
        }

        [Test]
        public void Fight_RefusesHeroesNotOwned()
        {
            var st = CampaignState.NewGame(G, 7, T0);
            string notOwned = InvariantTests.Roster.First(h => !st.Collection.Owns(h));
            Assert.Throws<ArgumentException>(() => st.Fight(G, 1, new[] { notOwned }, T0));
        }

        [Test]
        public void ChapterBoss_FirstClear_DropsOnePiece_OnlyOnce()
        {
            var st = CampaignState.NewGame(G, 7, T0);
            var team = st.Collection.Slots.Select(s => s.Hero).ToArray();
            st.Fight(G, 1, team, T0);
            Assert.AreEqual(0, st.Inventory.Items.Count, "ordinary stages drop no gear");
            var r = st.Fight(G, 2, team, T0);
            Assert.IsTrue(r.Won && r.FirstClear);
            Assert.AreEqual(1, st.Inventory.Items.Count);
            Assert.AreEqual(1, r.Gear.Count);
            Assert.AreEqual(1, r.Gear[0].Rarity, "only Uncommon is unlocked in chapter 1");
            st.Fight(G, 2, team, T0);
            Assert.AreEqual(1, st.Inventory.Items.Count, "no second drop on replays");
        }

        [Test]
        public void Collect_RollsIdleGear_FromAFullChest()
        {
            int pieces = 0;
            for (ulong seed = 1; seed <= 50; seed++)
            {
                var st = CampaignState.NewGame(G, seed, T0);
                st.HighestCleared = 1;
                st.Collect(G, T0 + 30 * 3600);
                int n = st.Inventory.Items.Count;
                Assert.IsTrue(n >= 1 && n <= 4, "a full chest gives 1-4 pieces");
                pieces += n;
            }
            Assert.AreEqual(1.68, pieces / 50.0, 0.4);
        }

        [Test]
        public void Power_CountsGear()
        {
            var st = CampaignState.NewGame(G, 7, T0);
            string h = st.Collection.Slots[0].Hero;
            double before = st.HeroPower(G, h);
            st.Inventory.Equip(G.Gear, st.Inventory.Add("brutal", "fury", 4), h);
            Assert.Greater(st.HeroPower(G, h), before);
        }

        [Test]
        public void SaveV2_RoundTripsCollectionSlotsGearAndDrops()
        {
            var st = CampaignState.NewGame(G, 7, T0);
            string h = st.Collection.Slots[2].Hero;
            st.Collection.Slots[2].Level = 44; st.Collection.Get(h).Stars = 4; st.Collection.Get(h).Copies = 3;
            st.Collection.AddSigils("ocean", 6);
            var item = st.Inventory.Add("charged", "vigor", 3); item.Upgrade = 7; st.Inventory.Equip(G.Gear, item, h);
            st.DropCounter = 12;
            var back = SaveGame.Read(SaveGame.Write(st), T0, G);
            Assert.AreEqual(44, back.Collection.Slots[2].Level);
            Assert.AreEqual(h, back.Collection.Slots[2].Hero);
            Assert.AreEqual(4, back.Collection.Get(h).Stars); Assert.AreEqual(3, back.Collection.Get(h).Copies);
            Assert.AreEqual(6, back.Collection.SigilsOf("ocean"));
            var bi = back.Inventory.Items.Single();
            Assert.AreEqual(("charged", "vigor", 3, 7, h), (bi.Type, bi.Set, bi.Rarity, bi.Upgrade, bi.EquippedOn));
            Assert.AreEqual(item.Id + 1, back.Inventory.NextId);
            Assert.AreEqual(12, back.DropCounter);
        }

        [Test]
        public void SaveV1_MigratesToAStarterTeam_KeepingProgress()
        {
            string v1 = "{\"version\":1,\"seed\":\"99\",\"highestCleared\":2,\"wallet\":{\"gold\":1234}}";
            var st = SaveGame.Read(v1, T0, G);
            Assert.AreEqual(2, st.HighestCleared);
            Assert.AreEqual(1234, st.Wallet.Get("gold"));
            Assert.AreEqual(5, st.Collection.Heroes.Count);
            CollectionAssert.AreEqual(Starter.Roll(G, 99), st.Collection.Slots.Select(s => s.Hero).ToList());
        }
    }
}
