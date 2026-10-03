using System;
using System.Collections.Generic;
using System.IO;
using Gacha.Core.Battle;
using Gacha.Core.Campaign;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Campaign state, first clears, auto mode and saves (decisions rows 18, 21, 34). Uses a 3-stage test campaign.</summary>
    [TestFixture]
    public class CampaignTests
    {
        const long T0 = 1_800_000_000;
        static readonly string[] Team = { "cassia" };

        // Stage 1: one weakling (always a win). Stage 2: an unkillable dummy (always a timeout loss). Stage 3: a weakling again.
        const string StagesJson = @"{ ""stages"": [
            { ""id"": ""ch01_s01"", ""index"": 1, ""chapter"": 1, ""stage"": 1, ""gate"": false, ""boss"": false, ""packages"": [""Burn""],
              ""enemies"": [ { ""hero"": ""weakling"", ""level"": 1, ""stars"": 1 } ], ""power"": 10, ""firstClear"": { ""gold"": 500, ""heroXp"": 200 } },
            { ""id"": ""ch01_s02"", ""index"": 2, ""chapter"": 1, ""stage"": 2, ""gate"": false, ""boss"": false, ""packages"": [],
              ""enemies"": [ { ""hero"": ""dummy"", ""level"": 1, ""stars"": 1 } ], ""power"": 10, ""firstClear"": { ""gold"": 600 } },
            { ""id"": ""ch01_s03"", ""index"": 3, ""chapter"": 1, ""stage"": 3, ""gate"": false, ""boss"": false, ""packages"": [],
              ""enemies"": [ { ""hero"": ""weakling"", ""level"": 2, ""stars"": 1 } ], ""power"": 10, ""firstClear"": { ""gold"": 700 } } ] }";

        static GameData _g;
        static GameData G
        {
            get
            {
                if (_g != null) return _g;
                _g = GameData.Load(TestData.Dir);
                _g.Heroes["dummy"] = TestData.Game.Heroes["dummy"];
                _g.Heroes["weakling"] = TestData.Dummy("weakling", "none", hp: 10, atk: 0, def: 0, atkSpd: 0);
                _g.Stages = StageDef.ListFromJson(StagesJson);
                return _g;
            }
        }

        static CampaignState Fresh() => CampaignState.New(seed: 42, now: T0);

        [Test]
        public void Expected_LevelAndStars()
        {
            Assert.AreEqual(1, Expected.Level(1));
            Assert.AreEqual(1, Expected.Level(2));
            Assert.AreEqual(10, Expected.Level(30));
            Assert.AreEqual(100, Expected.Level(300));
            Assert.AreEqual(200, Expected.Level(600));
            Assert.AreEqual(200, Expected.Level(700));
            Assert.AreEqual(1, Expected.Chapter(30)); Assert.AreEqual(2, Expected.Chapter(31)); Assert.AreEqual(20, Expected.Chapter(600));
            Assert.AreEqual(1, Expected.Stars(1)); Assert.AreEqual(1, Expected.Stars(4));
            Assert.AreEqual(2, Expected.Stars(5)); Assert.AreEqual(5, Expected.Stars(20));
        }

        [Test]
        public void StageDef_ReadsAllFields()
        {
            var s = G.Stages[0];
            Assert.AreEqual("ch01_s01", s.Id); Assert.AreEqual(1, s.Index); Assert.AreEqual(1, s.Chapter); Assert.AreEqual(1, s.Stage);
            Assert.IsFalse(s.Gate); Assert.IsFalse(s.Boss);
            Assert.AreEqual("Burn", s.Packages[0]);
            Assert.AreEqual("weakling", s.Enemies[0].Hero); Assert.AreEqual(1, s.Enemies[0].Level); Assert.AreEqual(1, s.Enemies[0].Stars);
            Assert.AreEqual(10, s.Power);
            Assert.AreEqual(500, s.FirstClear["gold"]); Assert.AreEqual(200, s.FirstClear["heroXp"]);
            var back = StageDef.ListFromJson(StageDef.ListToJson(G.Stages, pretty: true));
            Assert.AreEqual(3, back.Count); Assert.AreEqual(700, back[2].FirstClear["gold"]); Assert.AreEqual(2, back[2].Enemies[0].Level);
        }

        [Test]
        public void TeamPower_SumsScaledStats()
        {
            // cassia: HP 2400, ATK 160, DEF 140 -> 240 + 160 + 140 = 540 at level 1, 1 star.
            Assert.AreEqual(540, TeamPower.Of(G, Team, 1, 1));
            Assert.AreEqual(Math.Round(540 * 8.96 * 1.7), TeamPower.Of(G, Team, 200, 10));
        }

        [Test]
        public void Setups_UseExpectedLevelForPlayer_AndStageDataForEnemy()
        {
            var st = Fresh();
            var p = st.PlayerSetup(G, 300, Team);   // chapter 10 -> 3 stars, level 100
            Assert.AreEqual(G.Progression.Mult(100, 3), p.Scales[0], 1e-9);
            var e = st.EnemySetup(G, 3);
            Assert.AreEqual("weakling", e.Heroes[0]);
            Assert.AreEqual(G.Progression.Mult(2, 1), e.Scales[0], 1e-9);
        }

        [Test]
        public void Fight_NextStageWin_GrantsFirstClearOnce()
        {
            var st = Fresh();
            var r = st.Fight(G, 1, Team, T0 + 10);
            Assert.IsTrue(r.Won); Assert.IsTrue(r.FirstClear); Assert.AreEqual(1, r.StageIndex);
            Assert.AreEqual(1, st.HighestCleared);
            Assert.AreEqual(500, st.Wallet.Get("gold")); Assert.AreEqual(200, st.Wallet.Get("heroXp"));
            CollectionAssert.AreEqual(Team, st.LastSetup["campaign"]);
        }

        [Test]
        public void Replay_ClearedStage_NoSecondReward_NoRateChange()
        {
            var st = Fresh();
            st.Fight(G, 1, Team, T0 + 10);
            var r = st.Fight(G, 1, Team, T0 + 20);
            Assert.IsTrue(r.Won); Assert.IsFalse(r.FirstClear);
            Assert.AreEqual(1, st.HighestCleared);
            Assert.AreEqual(500, st.Wallet.Get("gold"));
        }

        [Test]
        public void Fight_Loss_ChangesNothingButAttempts()
        {
            var st = Fresh();
            st.Fight(G, 1, Team, T0 + 10);
            var r = st.Fight(G, 2, Team, T0 + 20);
            Assert.IsFalse(r.Won); Assert.IsFalse(r.FirstClear);
            Assert.AreEqual(1, st.HighestCleared); Assert.AreEqual(500, st.Wallet.Get("gold"));
            Assert.AreEqual(2, st.Attempts);
        }

        [Test]
        public void Fight_FirstClear_SettlesChestAtTheOldStage()
        {
            var st = Fresh();
            st.Fight(G, 1, Team, T0 + 3600);         // an hour with nothing cleared earns nothing
            st.Chest.Settle(T0 + 7200, st.HighestCleared, G.Idle);
            Assert.AreEqual(G.Idle.PerHour("gold", 1), st.Chest.Get("gold"), 1e-6);
            st.Collect(G, T0 + 7200);
            Assert.AreEqual(500 + G.Idle.PerHour("gold", 1), st.Wallet.Get("gold"), 1e-6);
        }

        [Test]
        public void Fight_SkippingAhead_Throws()
        {
            var st = Fresh();
            Assert.Throws<InvalidOperationException>(() => st.Fight(G, 2, Team, T0));
            Assert.Throws<InvalidOperationException>(() => st.Fight(G, 0, Team, T0));
            Assert.Throws<InvalidOperationException>(() => st.Fight(G, 4, Team, T0));
            Assert.Throws<ArgumentException>(() => st.Fight(G, 1, new string[0], T0));
            Assert.Throws<ArgumentException>(() => st.Fight(G, 1, new[] { "cassia", "cassia" }, T0));
            Assert.Throws<ArgumentException>(() => st.Fight(G, 1, new[] { "a", "b", "c", "d", "e", "f" }, T0));
            Assert.AreEqual(0, st.Attempts);
        }

        [Test]
        public void Fight_IsDeterministic_ForSameSaveSeed()
        {
            var a = Fresh(); var b = Fresh();
            var team = new[] { "cassia", "nyx", "halcyra" };
            var ra = a.Fight(G, 1, team, T0); var rb = b.Fight(G, 1, team, T0);
            Assert.AreEqual(ra.Seed, rb.Seed);
            Assert.AreEqual(ra.Battle.Hash, rb.Battle.Hash);
            var ra2 = a.Fight(G, 1, team, T0);
            Assert.AreNotEqual(ra.Seed, ra2.Seed);   // each attempt gets a new seed
        }

        [Test]
        public void Fight_Replay_ReproducesTheBattle()
        {
            var st = Fresh();
            var r = st.Fight(G, 1, new[] { "cassia", "nyx" }, T0);
            var again = new Battle(G, r.Player, r.Enemy, r.Seed).Run();
            Assert.AreEqual(r.Battle.Hash, again.Hash);
        }

        [Test]
        public void AutoContinues_StopsOnLossAndAtLastStage()
        {
            var st = Fresh();
            var win1 = st.Fight(G, 1, Team, T0);
            Assert.IsFalse(st.AutoContinues(G, win1));   // auto off
            st.Auto = true;
            Assert.IsTrue(st.AutoContinues(G, win1));
            var loss = st.Fight(G, 2, Team, T0);
            Assert.IsFalse(st.AutoContinues(G, loss));
            st.HighestCleared = 2;
            var last = st.Fight(G, 3, Team, T0);
            Assert.IsTrue(last.Won);
            Assert.IsFalse(st.AutoContinues(G, last));
        }

        [Test]
        public void Save_RoundTrips()
        {
            var st = CampaignState.New(seed: 0xFFFF_FFFF_FFFF_FFF1UL, now: T0);
            st.Fight(G, 1, new[] { "cassia", "nyx" }, T0 + 3600);
            st.Chest.Settle(T0 + 7200, st.HighestCleared, G.Idle);
            st.Auto = true;
            var back = SaveGame.Read(SaveGame.Write(st), T0 + 9999);
            Assert.AreEqual(st.Seed, back.Seed); Assert.AreEqual(st.Attempts, back.Attempts);
            Assert.AreEqual(1, back.HighestCleared); Assert.IsTrue(back.Auto);
            Assert.AreEqual(st.Wallet.Get("gold"), back.Wallet.Get("gold"));
            Assert.AreEqual(st.Chest.LastSettled, back.Chest.LastSettled);
            Assert.AreEqual(st.Chest.AccruedSeconds, back.Chest.AccruedSeconds);
            Assert.AreEqual(st.Chest.Get("gold"), back.Chest.Get("gold"));
            CollectionAssert.AreEqual(new[] { "cassia", "nyx" }, back.LastSetup["campaign"]);
        }

        [Test]
        public void Save_Corrupt_StartsFresh()
        {
            foreach (var bad in new[] { null, "", "{not json", "[1,2]", "{\"version\":1,\"highestCleared\":\"x\"}" })
            {
                var st = SaveGame.Read(bad, T0);
                Assert.AreEqual(0, st.HighestCleared, "input: " + bad);
                Assert.AreEqual(T0, st.Chest.LastSettled);
            }
        }

        [Test]
        public void Save_NewerVersion_Throws()
        {
            Assert.Throws<InvalidDataException>(() => SaveGame.Read("{\"version\": " + (SaveGame.Version + 1) + "}", T0));
        }
    }
}
