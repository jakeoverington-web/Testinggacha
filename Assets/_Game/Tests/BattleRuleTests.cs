using System.Linq;
using Gacha.Core.Battle;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Kind 1: one rule, one exact number. Dummies have crit 0, block 0, dodge 0, resist 0.</summary>
    [TestFixture]
    public class BattleRuleTests
    {
        static HitInfo Skill(string elem = null) => new HitInfo { Kind = HitKind.Skill, Elem = elem, Ability = "test" };

        [Test]
        public void Defence_UsesKOverKPlusDef()
        {
            var lab = Lab.Seed(1).Ally("striker").Enemy("dummy_def300").Run(5);
            var hits = lab.From(lab.A(), Ev.Damage).ToList();
            Assert.Greater(hits.Count, 2, "striker should have reached and hit the dummy");
            foreach (var h in hits) Assert.AreEqual(50.0 * lab.Battle.T.BasicPct, h.Amount, 1e-6, "100 ATK x basic % x 300/(300+300)");
        }

        [Test]
        public void CounterRace_Adds10Percent()
        {
            var lab = Lab.Seed(1).Ally("striker").Enemy("dummy_dark").Run(5);
            foreach (var h in lab.From(lab.A(), Ev.Damage)) Assert.AreEqual(110.0 * lab.Battle.T.BasicPct, h.Amount, 1e-6, "High beats Dark: +10%");
        }

        [Test]
        public void NoCounter_WhenRacesDoNotMatchTheLoop()
        {
            var lab = Lab.Seed(1).Ally("striker").Enemy("dummy").Run(5);
            foreach (var h in lab.From(lab.A(), Ev.Damage)) Assert.AreEqual(100.0 * lab.Battle.T.BasicPct, h.Amount, 1e-6);
        }

        [Test]
        public void BasicHit_Gives10Energy()
        {
            var lab = Lab.Seed(1).Ally("striker").Enemy("dummy").Run(6);
            int hits = lab.From(lab.A(), Ev.Damage).Count();
            Assert.AreEqual(hits * 10.0, lab.A().Energy, 1e-6);
        }

        [Test]
        public void EnergyFromDamageTaken_IsHalfPerPercentLost()
        {
            var b = Lab.Seed(1).Ally("dummy").Enemy("striker").Build();
            var t = b.Units[0]; t.Base.Hp = 1000; t.Hp = 1000;
            b.Hit(b.Units[1], t, 100, Skill());      // 10% of max HP
            Assert.AreEqual(5.0, t.Energy, 1e-6);
        }

        [Test]
        public void Soaked_LightningPlus25_FireMinus15()
        {
            var b = Lab.Seed(1).Ally("striker").Enemy("dummy").Build();
            var s = b.Units[0]; var t = b.Units[1];
            b.ApplyStatus(t, t, "soaked", 5, 0, null);   // self-sourced = never resisted
            Assert.AreEqual(125.0, b.Hit(s, t, 100, Skill("lightning")), 1e-6);
            Assert.AreEqual(85.0, b.Hit(s, t, 100, Skill("fire")), 1e-6);
        }

        [Test]
        public void MarkAndWeaken_Stack()
        {
            var b = Lab.Seed(1).Ally("striker").Enemy("dummy").Build();
            var t = b.Units[1];
            b.ApplyStatus(t, t, "mark", 5, 0.15, null);
            b.ApplyStatus(t, t, "weaken", 5, 0.25, null);
            Assert.AreEqual(140.0, b.Hit(b.Units[0], t, 100, Skill()), 1e-6);
        }

        [Test]
        public void Block_HalvesDamage()
        {
            var b = Lab.Seed(1).Ally("striker").Enemy("dummy").Build();
            b.Units[1].Base.Block = 1;
            Assert.AreEqual(50.0, b.Hit(b.Units[0], b.Units[1], 100, Skill()), 1e-6);
        }

        [Test]
        public void Shields_StackButCapAtHalfMaxHp()
        {
            var b = Lab.Seed(1).Ally("dummy").Enemy("dummy").Build();
            var t = b.Units[0];
            b.AddShield(t, t, t.MaxHp * 0.4, 0);
            b.AddShield(t, t, t.MaxHp * 0.4, 0);
            Assert.AreEqual(t.MaxHp * 0.5, t.ShieldTotal, 1e-6);
        }

        [Test]
        public void Shield_AbsorbsBeforeHp()
        {
            var b = Lab.Seed(1).Ally("dummy").Enemy("striker").Build();
            var t = b.Units[0];
            b.AddShield(t, t, 60, 0);
            b.Hit(b.Units[1], t, 100, Skill());
            Assert.AreEqual(0.0, t.ShieldTotal, 1e-6);
            Assert.AreEqual(t.MaxHp - 40, t.Hp, 1e-6);
        }

        [Test]
        public void Unkillable_KeepsOneHp()
        {
            var b = Lab.Seed(1).Ally("dummy").Enemy("striker").Build();
            var t = b.Units[0];
            b.ApplyStatus(t, t, "unkillable", 4, 0, null);
            b.Hit(b.Units[1], t, 10_000_000, Skill());
            Assert.IsTrue(t.Alive);
            Assert.AreEqual(1.0, t.Hp, 1e-9);
        }

        [Test]
        public void HardControl_CappedAt2_5s()
        {
            var b = Lab.Seed(1).Ally("striker").Enemy("dummy").Build();
            b.ResistFloor = 0;
            var t = b.Units[1];
            Assert.IsTrue(b.ApplyStatus(b.Units[0], t, "stun", 9, 0, null));
            Assert.AreEqual(2.5, t.Get("stun").Remaining, 1e-9);
        }

        [Test]
        public void HardControl_DiminishingReturns_FullThenHalfThenImmune()
        {
            var b = Lab.Seed(1).Ally("striker").Enemy("dummy").Build();
            b.ResistFloor = 0;
            var s = b.Units[0]; var t = b.Units[1];
            Assert.IsTrue(b.ApplyStatus(s, t, "stun", 1, 0, null));
            b.RunFor(1.5);
            Assert.IsTrue(b.ApplyStatus(s, t, "stun", 2, 0, null));
            Assert.AreEqual(1.0, t.Get("stun").Remaining, 1e-9, "second control within 4s lasts half");
            b.RunFor(1.5);
            Assert.IsFalse(b.ApplyStatus(s, t, "stun", 2, 0, null), "third control in the window is resisted");
            b.RunFor(5);
            Assert.IsTrue(b.ApplyStatus(s, t, "stun", 2, 0, null), "window over: full again");
        }

        [Test]
        public void HardControl_PriorityKeepsTheStronger()
        {
            var b = Lab.Seed(1).Ally("striker").Enemy("dummy").Build();
            b.ResistFloor = 0;
            var s = b.Units[0]; var t = b.Units[1];
            var t2 = b.Units[0];
            Assert.IsTrue(b.ApplyStatus(t, t, "charm", 2, 0, null));          // self-sourced, not hostile: no DR
            Assert.IsFalse(b.ApplyStatus(t, t, "stun", 2, 0, null), "stun is weaker than charm");
            Assert.IsTrue(t.Has("charm"));
        }

        [Test]
        public void ResistFloor_Is15Percent()
        {
            int landed = 0, n = 4000;
            var b = Lab.Seed(3).Ally("striker").Enemy("dummy").Build();
            for (int i = 0; i < n; i++)
            {
                if (b.ApplyStatus(b.Units[0], b.Units[1], "slow", 1, 0.3, null)) landed++;
                b.Units[1].Statuses.Clear();
            }
            Assert.AreEqual(0.85, landed / (double)n, 0.02);
        }

        [Test]
        public void Burn_StacksToThree()
        {
            var b = Lab.Seed(1).Ally("striker").Enemy("dummy").Build();
            var t = b.Units[1];
            for (int i = 0; i < 5; i++) b.ApplyStatus(t, t, "burn", 4, 0.3, null);
            Assert.AreEqual(3, t.Count("burn"));
        }

        [Test]
        public void Antiheal_ReducesHealing()
        {
            var b = Lab.Seed(1).Ally("dummy").Enemy("dummy").Build();
            var t = b.Units[0]; t.Hp = 100;
            b.ApplyStatus(t, t, "antiheal", 5, 0.5, null);
            Assert.AreEqual(50.0, b.Heal(t, t, 100, false), 1e-6);
        }

        [Test]
        public void Haste100_HalvesCooldowns()
        {
            var lab = Lab.Seed(1).Ally("valeria").Enemy("dummy");
            var b = lab.Build();
            var u = b.Units[0]; u.Base.Haste = 100;
            double before = u.Cd[1];
            b.RunFor(1.0);
            Assert.AreEqual(before - 2.0, u.Cd[1], 1e-6);
        }

        [Test]
        public void Taunt_ForcesTheTarget()
        {
            var b = Lab.Seed(1).Ally("striker").Enemy("dummy", "dummy_dark").Build();
            b.ResistFloor = 0;
            var s = b.Units[0]; var taunter = b.Units[2];
            b.ApplyStatus(taunter, s, "taunt", 2, 0, null);
            Assert.AreEqual(taunter, b.EnemyTarget(s));
        }

        [Test]
        public void Shields_TrimmedWhenMaxHpShrinks()
        {
            var b = Lab.Seed(1).Ally("draxa").Enemy("dummy").Build();
            var d = b.Units[0];
            b.ApplyStatus(d, d, "dragonform", 0.5, 0.5, null);
            b.AddShield(d, d, d.MaxHp, 0);                      // fills the bigger cap
            b.RunFor(1.0);                                      // dragonform ends
            Assert.LessOrEqual(d.ShieldTotal, b.T.ShieldCap * d.MaxHp + 1e-6);
        }

        [Test]
        public void ReviveOnlyOnce()
        {
            var b = Lab.Seed(1).Ally("dummy").Enemy("dummy").Build();
            var t = b.Units[0];
            b.Kill(t, null);
            Assert.IsTrue(b.Revive(t, t, 0.4));
            Assert.AreEqual(t.MaxHp * 0.4, t.Hp, 1e-6);
            b.Kill(t, null);
            Assert.IsFalse(b.Revive(t, t, 0.4));
        }

        [Test]
        public void TimeLimit_EndsAsLossForAttacker()
        {
            var r = new Battle(Lab.Data, new TeamSetup("dummy"), new TeamSetup("dummy"), 1).Run();
            Assert.AreEqual(-1, r.Winner);
            Assert.AreEqual(90.0, r.Time, 1e-6);
        }

        [Test]
        public void SameSeed_SameBattle_DifferentSeed_DifferentBattle()
        {
            var a = new TeamSetup("valeria", "ilyra", "aurelle", "seravelle", "lucienne");
            var b = new TeamSetup("sangrael", "nyx", "mordessa", "liora", "ravenna");
            var r1 = new Battle(Lab.Data, a, b, 7).Run();
            var r2 = new Battle(Lab.Data, a, b, 7).Run();
            var r3 = new Battle(Lab.Data, a, b, 8).Run();
            Assert.AreEqual(r1.Hash, r2.Hash);
            Assert.AreNotEqual(r1.Hash, r3.Hash);
        }

        [Test]
        public void TeamBonus_FiveOfOneCore_AppliesWhenTuned()
        {
            var data = Gacha.Core.Battle.GameData.Load(TestData.Dir);
            data.BonusFiveOfOne = new System.Collections.Generic.Dictionary<string, double> { { "hp", 0.2 } };
            var b = new Battle(data, new TeamSetup("valeria", "ilyra", "aurelle", "seravelle", "lucienne"), new TeamSetup("sangrael"), 1);
            var v = b.Units[0];
            Assert.AreEqual(v.Def.Stats.Hp * data.Tuning.HpScale * 1.2, v.MaxHp, 1e-6);
            Assert.AreEqual(0.0, b.Units[5].BonusHpPct, 1e-9, "a lone hero gets no bonus");
        }
    }
}
