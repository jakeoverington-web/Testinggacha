using System.Linq;
using Gacha.Core.Battle;
using Gacha.Core.Campaign;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>After-fight statistics: damage dealt, healing done, damage taken per hero (owner, 2026-10-03).</summary>
    [TestFixture]
    public class FightStatsTests
    {
        static Battle Played(TeamSetup a, TeamSetup b, ulong seed = 3)
        {
            var battle = new Battle(Lab.Data, a, b, seed);
            battle.Run();
            return battle;
        }

        [Test]
        public void Totals_MatchTheLog_InFieldOrder()
        {
            var b = Played(new TeamSetup("cassia", "nyx", "aurelle"), new TeamSetup("sangrael", "isolde", "halcyra"));
            var lines = FightStats.From(b);
            CollectionAssert.AreEqual(new[] { "cassia", "nyx", "aurelle" }, lines.Where(l => l.Team == 0).Select(l => l.Hero).ToArray(), "field order");
            CollectionAssert.AreEqual(new[] { "sangrael", "isolde", "halcyra" }, lines.Where(l => l.Team == 1).Select(l => l.Hero).ToArray());
            double dealtByA = lines.Where(l => l.Team == 0).Sum(l => l.Dealt);
            double takenByB = lines.Where(l => l.Team == 1).Sum(l => l.Taken);
            Assert.Greater(dealtByA, 0);
            Assert.AreEqual(dealtByA, takenByB, 1e-6, "everything team A dealt, team B took (summons included)");
            Assert.Greater(lines.First(l => l.Hero == "aurelle").Healed, 0, "the healer heals");
        }

        [Test]
        public void FallenHeroes_KeepTheirStats_AndAreMarked()
        {
            var b = Played(new TeamSetup("cassia", "nyx", "aurelle"), new TeamSetup("sangrael", "isolde", "halcyra"));
            var lines = FightStats.From(b);
            var fallen = lines.Where(l => !l.Alive).ToList();
            Assert.Greater(fallen.Count, 0, "someone falls in this fight");
            foreach (var l in fallen)
            {
                Assert.Greater(l.Taken, 0, l.Hero + " took damage before falling");
                Assert.IsFalse(b.Units.First(u => u.Id == l.Hero && u.Team == l.Team && u.IsHero).Alive);
            }
        }

        [Test]
        public void ShieldedDamage_CountsAsDealtAndTaken()
        {
            var b = new Battle(Lab.Data, new TeamSetup("striker"), new TeamSetup("dummy"), 1);
            var dummy = b.Units[1];
            b.AddShield(dummy, dummy, 1e6, 0);
            b.RunFor(5);
            var lines = FightStats.From(b);
            Assert.Greater(lines.First(l => l.Hero == "striker").Dealt, 0, "hits on a shield still count");
            Assert.AreEqual(lines.First(l => l.Hero == "striker").Dealt, lines.First(l => l.Hero == "dummy").Taken, 1e-6);
            Assert.AreEqual(dummy.MaxHp, dummy.Hp, 1e-6, "the shield took it all");
        }

        [Test]
        public void SummonsCountForTheirOwner()
        {
            string summoner = InvariantTests.Roster.First(id =>
            {
                var d = Lab.Data.Heroes[id];
                return new[] { d.Ult, d.S1, d.S2 }.Any(ab => ab.Ops.Any(o => o.Str("op") == "summon"));
            });
            var b = Played(new TeamSetup(summoner, "cassia"), new TeamSetup("sangrael", "isolde"), 7);
            var lines = FightStats.From(b);
            Assert.AreEqual(2, lines.Count(l => l.Team == 0), "no separate line for summons");
            double summonDamage = b.Log.Where(e => e.Type == Ev.Damage && e.Src >= 0 && b.Units[e.Src].IsSummon && b.Units[e.Dst].Team == 1).Sum(e => e.Amount);
            double heroOwn = b.Log.Where(e => e.Type == Ev.Damage && e.Src >= 0 && b.Units[e.Src].Id == summoner && !b.Units[e.Src].IsSummon && b.Units[e.Dst].Team == 1).Sum(e => e.Amount);
            Assert.AreEqual(heroOwn + summonDamage, lines.First(l => l.Hero == summoner).Dealt, 1e-6);
        }

        [Test]
        public void SelfDamage_IsNotDamageTaken()
        {
            var b = new Battle(Lab.Data, new TeamSetup("striker"), new TeamSetup("dummy"), 1);
            b.Units[0].Hp -= 0;   // no-op; self damage arrives as a Damage event with Src == Dst
            b.Log.Add(new BattleEvent { Type = Ev.Damage, Src = 0, Dst = 0, Amount = 500, What = "spend_hp" });
            var lines = FightStats.From(b);
            Assert.AreEqual(0, lines.First(l => l.Hero == "striker").Taken);
            Assert.AreEqual(0, lines.First(l => l.Hero == "striker").Dealt);
        }

        [Test]
        public void FinishFight_AttachesStats()
        {
            var st = CampaignState.New(1, 0);
            var r = st.Fight(Lab.Data, 1, new[] { "cassia", "nyx" }, 0);
            Assert.AreEqual(2, r.Stats.Count(l => l.Team == 0));
            Assert.AreEqual(Lab.Data.Stages[0].Enemies.Count, r.Stats.Count(l => l.Team == 1));
        }
    }
}
