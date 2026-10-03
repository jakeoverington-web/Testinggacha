using System.Collections.Generic;
using System.IO;
using Gacha.Core.Battle;
using Gacha.Core.Progression;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Level and star stat scaling (decisions rows 31, 34; progression.json).</summary>
    [TestFixture]
    public class ProgressionTests
    {
        static StatScaling S => StatScaling.FromJson(File.ReadAllText(Path.Combine(TestData.Dir, "progression.json")));

        [Test]
        public void LevelMult_GrowsFourPercentPerLevel()
        {
            Assert.AreEqual(1.0, S.LevelMult(1), 1e-9);
            Assert.AreEqual(1.04, S.LevelMult(2), 1e-9);
            Assert.AreEqual(8.96, S.LevelMult(200), 1e-9);
            Assert.AreEqual(200, S.MaxLevel);
        }

        [Test]
        public void StarMult_MatchesTable()
        {
            Assert.AreEqual(1.00, S.StarMult(1), 1e-9);
            Assert.AreEqual(1.20, S.StarMult(5), 1e-9);
            Assert.AreEqual(1.30, S.StarMult(6), 1e-9);
            Assert.AreEqual(1.70, S.StarMult(10), 1e-9);
            Assert.AreEqual(8.96 * 1.70, S.Mult(200, 10), 1e-9);
        }

        [Test]
        public void StarAndLevel_OutOfRange_Clamp()
        {
            Assert.AreEqual(S.LevelMult(1), S.LevelMult(0), 1e-9);
            Assert.AreEqual(S.LevelMult(200), S.LevelMult(250), 1e-9);
            Assert.AreEqual(S.StarMult(1), S.StarMult(0), 1e-9);
            Assert.AreEqual(1.70, S.StarMult(11), 1e-9);
        }

        [Test]
        public void GameData_LoadsProgression()
        {
            Assert.IsNotNull(Lab.Data.Progression);
            Assert.AreEqual(8.96, Lab.Data.Progression.LevelMult(200), 1e-9);
        }

        [Test]
        public void TeamScales_MultiplyBaseStats()
        {
            var plain = new Battle(Lab.Data, new TeamSetup("cassia", "nyx"), new TeamSetup("dummy"), 1);
            var scaled = new Battle(Lab.Data, new TeamSetup("cassia", "nyx") { Scales = new List<double> { 2.0, 3.0 } }, new TeamSetup("dummy"), 1);
            Assert.AreEqual(plain.Units[0].Base.Hp * 2, scaled.Units[0].Base.Hp, 1e-6);
            Assert.AreEqual(plain.Units[0].Base.Atk * 2, scaled.Units[0].Base.Atk, 1e-6);
            Assert.AreEqual(plain.Units[1].Base.Def * 3, scaled.Units[1].Base.Def, 1e-6);
            Assert.AreEqual(plain.Units[2].Base.Hp, scaled.Units[2].Base.Hp, 1e-6);   // enemy untouched
        }
    }
}
