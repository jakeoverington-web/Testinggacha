using System.Collections.Generic;
using System.Linq;
using Gacha.Core.Battle;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Per-hero stat bonuses (gear, sets) and ability ranks from stars (phase 2 spec). Defaults change nothing.</summary>
    [TestFixture]
    public class BonusAndRankTests
    {
        static Battle With(string hero, StatBonus bonus = null, int[] ranks = null)
        {
            var a = new TeamSetup(hero);
            if (bonus != null) a.Bonuses = new List<StatBonus> { bonus };
            if (ranks != null) a.Ranks = new List<int[]> { ranks };
            return new Battle(Lab.Data, a, new TeamSetup("dummy"), 5) { ResistFloor = 0 };
        }

        [Test]
        public void Bonus_LandsOnTheUnit()
        {
            var plain = With("nyx").Units[0].Base;
            var b = new StatBonus { HpPct = 0.4, AtkPct = 0.3, DefPct = 0.2, Haste = 20, AtkSpdPct = 0.15, MoveSpdPct = 0.2, CritRate = 0.15, CritDmg = 0.3,
                Accuracy = 0.15, Dodge = 0.12, Block = 0.15, EffectHit = 0.2, EffectRes = 0.2, Lifesteal = 0.12, HealPower = 0.2, HealRecv = 0.2, EnergyRegen = 0.2 };
            var s = With("nyx", b).Units[0].Base;
            Assert.AreEqual(plain.Hp * 1.4, s.Hp, 1e-6);
            Assert.AreEqual(plain.Atk * 1.3, s.Atk, 1e-6);
            Assert.AreEqual(plain.Def * 1.2, s.Def, 1e-6);
            Assert.AreEqual(plain.Haste + 20, s.Haste, 1e-9);
            Assert.AreEqual(plain.AtkSpd * 1.15, s.AtkSpd, 1e-9);
            Assert.AreEqual(plain.MoveSpd * 1.2, s.MoveSpd, 1e-9);
            Assert.AreEqual(plain.CritRate + 0.15, s.CritRate, 1e-9);
            Assert.AreEqual(plain.CritDmg + 0.3, s.CritDmg, 1e-9);
            Assert.AreEqual(plain.EffectRes + 0.2, s.EffectRes, 1e-9);
            Assert.AreEqual(plain.HealRecv + 0.2, s.HealRecv, 1e-9);
            Assert.AreEqual(plain.EnergyRegen + 0.2, s.EnergyRegen, 1e-9);
        }

        static string HeroWithDamageSkill() => InvariantTests.Roster.First(id =>
        {
            var ops = Lab.Data.Heroes[id].S1.Ops;
            return ops.Count == 1 && ops[0].Str("op") == "dmg" && !ops[0].Has("then");
        });

        [Test]
        public void Rank2_AbilityDealsTenPercentMore_Rank3Twenty()
        {
            string hero = HeroWithDamageSkill();
            double Loss(int rank)
            {
                var b = With(hero, null, new[] { 1, rank, 1, 1 });
                b.DisablePassives = true;
                var dummy = b.Units[1]; double before = dummy.Hp;
                b.ForceCast(b.Units[0], "s1");
                return before - dummy.Hp;
            }
            double r1 = Loss(1);
            Assert.Greater(r1, 0, hero);
            Assert.AreEqual(r1 * 1.1, Loss(2), r1 * 1e-9);
            Assert.AreEqual(r1 * 1.2, Loss(3), r1 * 1e-9);
        }

        [Test]
        public void StartEnergy_AndShieldReceived()
        {
            var b = With("nyx", new StatBonus { StartEnergy = 30, ShieldRecv = 0.2 });
            b.Step();
            Assert.GreaterOrEqual(b.Units[0].Energy, 30);
            var plain = With("nyx");
            plain.AddShield(plain.Units[0], plain.Units[0], 100, 0);
            b.AddShield(b.Units[0], b.Units[0], 100, 0);
            Assert.AreEqual(plain.Units[0].Shields[0].Amount * 1.2, b.Units[0].Shields[0].Amount, 1e-6);
        }

        [Test]
        public void NullBonusesAndRanks_ChangeNothing()
        {
            var a = new Battle(Lab.Data, new TeamSetup("nyx", "cassia"), new TeamSetup("sangrael", "isolde"), 9).Run();
            var t = new TeamSetup("nyx", "cassia") { Bonuses = new List<StatBonus> { null, new StatBonus() }, Ranks = new List<int[]> { new[] { 1, 1, 1, 1 }, null } };
            var b = new Battle(Lab.Data, t, new TeamSetup("sangrael", "isolde"), 9).Run();
            Assert.AreEqual(a.Hash, b.Hash);
        }
    }
}
