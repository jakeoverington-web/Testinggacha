using System.Collections.Generic;
using System.Globalization;
using Gacha.Core.Data;
using NUnit.Framework;

namespace Gacha.Tests
{
    /// <summary>Json.Write: saves and generated data files must read back exactly.</summary>
    [TestFixture]
    public class JsonTests
    {
        [Test]
        public void Write_ThenParse_RoundTrips()
        {
            var src = new Dictionary<string, object>
            {
                ["name"] = "Say \"hi\"\nback\\slash",
                ["n"] = 0.1,
                ["big"] = 490000.0,
                ["neg"] = -3.0,
                ["ok"] = true,
                ["none"] = null,
                ["list"] = new List<object> { 1.0, "two", new Dictionary<string, object> { ["k"] = false } },
                ["empty"] = new List<object>(),
            };
            foreach (var pretty in new[] { false, true })
            {
                var back = (Dictionary<string, object>)Json.Parse(Json.Write(src, pretty));
                Assert.AreEqual(src["name"], back["name"]);
                Assert.AreEqual(0.1, back["n"]);
                Assert.AreEqual(490000.0, back["big"]);
                Assert.AreEqual(-3.0, back["neg"]);
                Assert.AreEqual(true, back["ok"]);
                Assert.IsNull(back["none"]);
                var l = (List<object>)back["list"];
                Assert.AreEqual(1.0, l[0]); Assert.AreEqual("two", l[1]);
                Assert.AreEqual(false, ((Dictionary<string, object>)l[2])["k"]);
                Assert.AreEqual(0, ((List<object>)back["empty"]).Count);
            }
        }

        [Test]
        public void Write_IntegersAndLongs_HaveNoDecimals()
        {
            Assert.AreEqual("[3,5000000000,2]", Json.Write(new List<object> { 3, 5000000000L, 2.0 }));
        }

        [Test]
        public void Write_UsesInvariantCulture()
        {
            var old = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                Assert.AreEqual("[1.5]", Json.Write(new List<object> { 1.5 }));
            }
            finally { CultureInfo.CurrentCulture = old; }
        }

        [Test]
        public void Write_EscapesControlChars()
        {
            var s = Json.Write("a\tb\u0001");
            Assert.AreEqual("\"a\\tb\\u0001\"", s);
            Assert.AreEqual("a\tb\u0001", Json.Parse(s));
        }
    }
}
