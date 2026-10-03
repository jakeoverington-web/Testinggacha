using System.Linq;
using Gacha.Core.Battle;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Optional manual ultimates and the replayable input log (redesign spec; decisions row 1 change).</summary>
    [TestFixture]
    public class BattleInputTests
    {
        const string Hero = "nyx";

        static Lab Manual(out Battle b)
        {
            var lab = Lab.Seed(3).Ally(Hero).Enemy("dummy");
            b = lab.Build();
            b.ManualUltimates[0] = true;
            lab.A().Energy = 100;
            return lab;
        }

        static int UltCasts(Lab lab) => lab.From(lab.A(), Ev.Cast).Count(e => e.What == "ult");

        static BattleInput Cast(int unit) => new BattleInput { Kind = InputKind.CastUltimate, Unit = unit };

        [Test]
        public void Manual_NeverAutoCasts()
        {
            var lab = Manual(out var b);
            b.RunFor(30);
            Assert.AreEqual(0, UltCasts(lab));
            Assert.GreaterOrEqual(lab.A().Energy, 100 - 1e-9);
        }

        [Test]
        public void Manual_QueuedCastFiresNextTick()
        {
            var lab = Manual(out var b);
            b.Step();
            Assert.AreEqual(0, UltCasts(lab));
            b.Queue(Cast(lab.A().Index));
            b.Step();
            Assert.AreEqual(1, UltCasts(lab));
            Assert.AreEqual(1, b.Inputs.Count);
            Assert.AreEqual(2, b.Inputs[0].Tick);
        }

        [Test]
        public void Manual_RequestWaitsThroughStun()
        {
            var lab = Manual(out var b);
            var u = lab.A();
            u.Statuses.Add(new Status { Id = "stun", Remaining = 1.0, Source = lab.B().Index });
            b.Queue(Cast(u.Index));
            b.RunFor(0.5);
            Assert.AreEqual(0, UltCasts(lab), "stunned: the cast waits");
            b.RunFor(1.5);
            Assert.AreEqual(1, UltCasts(lab), "fires once the stun ends, exactly once");
        }

        [Test]
        public void Manual_IllegalRequestsDropped()
        {
            var lab = Manual(out var b);
            b.Queue(Cast(lab.B().Index));                       // enemy unit
            b.Queue(Cast(99));                                  // no such unit
            b.Step();
            Assert.AreEqual(0, b.Inputs.Count);
            b.ManualUltimates[0] = false;                       // auto team: requests are meaningless
            lab.A().Energy = 0;
            b.Queue(Cast(lab.A().Index));
            b.Step();
            Assert.AreEqual(0, b.Inputs.Count);
            lab.A().Alive = false;                              // dead unit
            b.ManualUltimates[0] = true;
            b.Queue(Cast(lab.A().Index));
            b.Step();
            Assert.AreEqual(0, b.Inputs.Count);
        }

        [Test]
        public void SetAuto_MidFight_CastsOnce()
        {
            var lab = Manual(out var b);
            b.Queue(Cast(lab.A().Index));
            b.Queue(new BattleInput { Kind = InputKind.SetAuto, Team = 0, On = true });
            b.RunFor(1.0);
            Assert.AreEqual(1, UltCasts(lab));
            Assert.IsFalse(b.ManualUltimates[0]);
            Assert.AreEqual(2, b.Inputs.Count);
        }

        [Test]
        public void Replay_WithInputs_SameHash()
        {
            var a = new TeamSetup("nyx", "cassia", "halcyra"); var e = new TeamSetup("sangrael", "isolde", "aurelle");
            var b = new Battle(Lab.Data, a, e, 77);
            b.ManualUltimates[0] = true;
            while (!b.Over)
            {
                foreach (var u in b.Units.Where(u => u.Team == 0 && u.IsHero && u.Alive && u.Energy >= 100 - 1e-9 && !u.UltRequested))
                    b.Queue(Cast(u.Index));
                b.Step();
            }
            Assert.Greater(b.Inputs.Count, 0, "the fight must have used inputs");
            var original = b.Result();
            var replay = Battle.Replay(Lab.Data, a, e, 77, new[] { true, false }, b.Inputs);
            Assert.AreEqual(original.Hash, replay.Hash);
            Assert.AreEqual(original.Winner, replay.Winner);
        }
    }
}
