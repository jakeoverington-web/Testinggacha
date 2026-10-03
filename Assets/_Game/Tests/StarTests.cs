using System.Collections.Generic;
using Gacha.Core.Economy;
using Gacha.Core.Progression;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Star ranks, fodder, Race Sigils and free resets (decisions rows 2, 3, 5, 31, 32; phase 2 spec).</summary>
    [TestFixture]
    public class StarTests
    {
        static Gacha.Core.Battle.GameData G => Lab.Data;
        // Lumarin (high): cassia, isolde, valeria. Noctyr (dark): nyx.
        static Wallet Gold(double g) { var w = new Wallet(); w.Add("gold", g); return w; }
        static readonly List<string> None = new List<string>();

        [Test]
        public void Raise_TwoStar_SpendsOneCopyAndGold()
        {
            var c = new Collection(); c.Add("cassia", 2); var w = Gold(1e9);
            Assert.IsTrue(Stars.Raise(G, c, "cassia", None, 0, w));
            Assert.AreEqual(2, c.Get("cassia").Stars);
            Assert.AreEqual(0, c.Get("cassia").Copies);
            Assert.AreEqual(1e9 - 25000, w.Get("gold"));
        }

        [Test]
        public void Raise_RefusedWhenShortOfCopiesOrGold_OrAtTen()
        {
            var c = new Collection(); c.Add("cassia");
            Assert.IsNotNull(Stars.Why(G, c, "cassia", None, 0, Gold(1e9)), "no copy");
            c.Add("cassia");
            Assert.IsNotNull(Stars.Why(G, c, "cassia", None, 0, Gold(10)), "no gold");
            c.Get("cassia").Stars = 10;
            Assert.IsNotNull(Stars.Why(G, c, "cassia", None, 0, Gold(1e9)), "already 10");
        }

        [Test]
        public void Fodder_MustBeSameRaceOneStarAndFree()
        {
            var c = new Collection(); c.Add("cassia", 2); c.Get("cassia").Stars = 5; c.Get("cassia").Copies = 1;  // next: 6 = 1 copy + 2 fodder
            c.Add("isolde"); c.Add("valeria"); c.Add("nyx");
            var w = Gold(1e9);
            Assert.IsNotNull(Stars.Why(G, c, "cassia", new List<string> { "isolde", "nyx" }, 0, w), "wrong race");
            c.Get("valeria").Stars = 2;
            Assert.IsNotNull(Stars.Why(G, c, "cassia", new List<string> { "isolde", "valeria" }, 0, w), "starred fodder must be reset first");
            c.Get("valeria").Stars = 1;
            c.Contract(0, "valeria");
            Assert.IsNotNull(Stars.Why(G, c, "cassia", new List<string> { "isolde", "valeria" }, 0, w), "a Contract hero cannot be fed");
            c.Uncontract(0);
            Assert.IsNotNull(Stars.Why(G, c, "cassia", new List<string> { "isolde", "valeria" }, 0, w, locked: id => id == "valeria"), "a geared hero cannot be fed");
            Assert.IsNull(Stars.Why(G, c, "cassia", new List<string> { "isolde", "valeria" }, 0, w));
            Assert.IsTrue(Stars.Raise(G, c, "cassia", new List<string> { "isolde", "valeria" }, 0, w));
            Assert.AreEqual(6, c.Get("cassia").Stars);
            Assert.IsFalse(c.Owns("isolde")); Assert.IsFalse(c.Owns("valeria"));
        }

        [Test]
        public void Fodder_SpareCopyIsUsedBeforeTheHero()
        {
            var c = new Collection(); c.Add("cassia", 2); c.Get("cassia").Stars = 5; c.Get("cassia").Copies = 1;
            c.Add("isolde", 3);                                  // isolde + 2 spare copies
            Assert.IsTrue(Stars.Raise(G, c, "cassia", new List<string> { "isolde", "isolde" }, 0, Gold(1e9)));
            Assert.IsTrue(c.Owns("isolde")); Assert.AreEqual(0, c.Get("isolde").Copies);
        }

        [Test]
        public void Sigils_CountAsFodder()
        {
            var c = new Collection(); c.Add("cassia", 2); c.Get("cassia").Stars = 5; c.Get("cassia").Copies = 1;
            c.AddSigils("high", 1); c.Add("isolde");
            Assert.IsNotNull(Stars.Why(G, c, "cassia", None, 2, Gold(1e9)), "only 1 Sigil owned");
            Assert.IsTrue(Stars.Raise(G, c, "cassia", new List<string> { "isolde" }, 1, Gold(1e9)));
            Assert.AreEqual(0, c.SigilsOf("high"));
        }

        [Test]
        public void Reset_RefundsEverything_AsCopiesGoldAndSigils()
        {
            var c = new Collection(); c.Add("cassia", 9); c.AddSigils("high", 15); var w = Gold(1e9);
            for (int s = 2; s <= 10; s++)
                Assert.IsTrue(Stars.Raise(G, c, "cassia", None, G.StarCosts.For(s).Fodder, w), "to " + s);
            Assert.AreEqual(10, c.Get("cassia").Stars);
            Stars.Reset(G, c, "cassia", w);
            Assert.AreEqual(1, c.Get("cassia").Stars);
            Assert.AreEqual(8, c.Get("cassia").Copies);
            Assert.AreEqual(15, c.SigilsOf("high"));
            Assert.AreEqual(1e9, w.Get("gold"), 1e-6, "value never grows or shrinks");
        }
    }
}
