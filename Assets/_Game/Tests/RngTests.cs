using Gacha.Core;
using NUnit.Framework;

namespace Gacha.Tests
{
    [TestFixture]
    public class RngTests
    {
        [Test]
        public void SameSeedGivesSameSequence()
        {
            var a = new Rng(42); var b = new Rng(42);
            for (int i = 0; i < 1000; i++) Assert.AreEqual(a.NextULong(), b.NextULong());
        }

        [Test]
        public void KnownFirstValue_PinsTheAlgorithm()
        {
            // SplitMix64 reference output for seed 0. If this changes, every saved replay breaks.
            Assert.AreEqual(0xE220A8397B1DCDAFUL, new Rng(0).NextULong());
        }

        [Test]
        public void DoubleStaysInUnitRange_AndChanceMatchesProbability()
        {
            var r = new Rng(7); int hits = 0;
            for (int i = 0; i < 20000; i++) { var d = r.NextDouble(); Assert.IsTrue(d >= 0 && d < 1); if (r.Chance(0.25)) hits++; }
            Assert.AreEqual(0.25, hits / 20000.0, 0.02);
        }
    }
}
