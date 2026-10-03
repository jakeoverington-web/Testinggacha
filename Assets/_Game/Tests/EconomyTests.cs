using System;
using System.IO;
using Gacha.Core.Economy;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Idle chest maths (decisions row 33: 24 h hard cap; Game Modes and Progression Plan, Idle rewards).</summary>
    [TestFixture]
    public class EconomyTests
    {
        static IdleRates R => IdleRates.FromJson(File.ReadAllText(Path.Combine(TestData.Dir, "economy.json")));
        const long T0 = 1_800_000_000;   // any fixed unix time
        const long H = 3600;

        [Test]
        public void Rates_MatchPlanTable()
        {
            var r = R;
            CollectionAssert.AreEqual(new[] { "gold", "heroXp", "starlight" }, r.Resources);
            Assert.AreEqual(7000, r.PerHour("gold", 30), 1e-6);
            Assert.AreEqual(3000, r.PerHour("heroXp", 30), 1e-6);
            Assert.AreEqual(24, r.PerHour("starlight", 30), 1e-6);
            Assert.AreEqual(490_000, r.PerHour("gold", 600), 1e-6);
            Assert.AreEqual(210_000, r.PerHour("heroXp", 600), 1e-6);
            Assert.AreEqual(720, r.PerHour("starlight", 600), 1e-6);
            Assert.AreEqual(7000 * Math.Pow(70, 1.0 / 570), r.PerHour("gold", 31), 1e-6);   // about 0.75% a stage
            Assert.AreEqual(0, r.PerHour("gold", 0));
            Assert.AreEqual(24, r.CapHours); Assert.AreEqual(2, r.HourglassHours);
            Assert.AreEqual(0, R.PerHour("unknown", 30));
        }

        [Test]
        public void Chest_FillsLinearly()
        {
            var c = IdleChest.StartAt(T0);
            c.Settle(T0 + H, 30, R);
            Assert.AreEqual(7000, c.Held["gold"], 1e-6);
            Assert.AreEqual(3000, c.Held["heroXp"], 1e-6);
        }

        [Test]
        public void Chest_StopsAt24h()
        {
            var c = IdleChest.StartAt(T0);
            c.Settle(T0 + 5 * 24 * H, 30, R);
            Assert.AreEqual(24 * 7000, c.Held["gold"], 1e-6);
            c.Settle(T0 + 6 * 24 * H, 30, R);
            Assert.AreEqual(24 * 7000, c.Held["gold"], 1e-6);
        }

        [Test]
        public void Chest_ClockBackwards_AddsNothing()
        {
            var c = IdleChest.StartAt(T0);
            c.Settle(T0 + H, 30, R);
            c.Settle(T0, 30, R);            // phone clock set back an hour
            c.Settle(T0 + H, 30, R);        // and forward again
            Assert.AreEqual(7000, c.Held["gold"], 1e-6);
            c.Settle(T0 + 2 * H, 30, R);    // real time moves on
            Assert.AreEqual(14000, c.Held["gold"], 1e-6);
        }

        [Test]
        public void Chest_RateChange_SplitsAtSettle()
        {
            var c = IdleChest.StartAt(T0);
            c.Settle(T0 + H, 30, R);
            c.Settle(T0 + 2 * H, 31, R);
            Assert.AreEqual(7000 + R.PerHour("gold", 31), c.Held["gold"], 1e-6);
        }

        [Test]
        public void Chest_NothingClearedYet_DoesNotUseUpCap()
        {
            var c = IdleChest.StartAt(T0);
            c.Settle(T0 + 30 * H, 0, R);    // a day and more before the first clear
            Assert.AreEqual(0, c.Get("gold"));
            c.Settle(T0 + 31 * H, 30, R);
            Assert.AreEqual(7000, c.Get("gold"), 1e-6);
        }

        [Test]
        public void Collect_MovesLootAndResetsCap()
        {
            var c = IdleChest.StartAt(T0); var w = new Wallet();
            c.Settle(T0 + 30 * H, 30, R);
            c.Collect(T0 + 30 * H, 30, R, w);
            Assert.AreEqual(24 * 7000, w.Get("gold"), 1e-6);
            Assert.AreEqual(0, c.Get("gold"));
            c.Settle(T0 + 31 * H, 30, R);
            Assert.AreEqual(7000, c.Get("gold"), 1e-6);
        }

        [Test]
        public void Hourglass_PaysTwoHours_IgnoringCap()
        {
            var c = IdleChest.StartAt(T0); var w = new Wallet();
            c.Settle(T0 + 48 * H, 30, R);   // chest already full
            c.UseHourglass(30, R, w);
            Assert.AreEqual(2 * 7000, w.Get("gold"), 1e-6);
            Assert.AreEqual(24 * 7000, c.Get("gold"), 1e-6);
        }

        [Test]
        public void GameData_LoadsIdleRates()
        {
            Assert.IsNotNull(Lab.Data.Idle);
            Assert.AreEqual(7000, Lab.Data.Idle.PerHour("gold", 30), 1e-6);
        }

        [Test]
        public void Wallet_AddsAndReadsZeroForUnknown()
        {
            var w = new Wallet();
            Assert.AreEqual(0, w.Get("gold"));
            w.Add("gold", 5); w.Add("gold", 2.5);
            Assert.AreEqual(7.5, w.Get("gold"));
            Assert.Throws<ArgumentException>(() => w.Add("gold", double.NaN));
        }
    }
}
