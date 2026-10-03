using System.Collections.Generic;
using Gacha.Core.Data;

namespace Gacha.Core.Gear
{
    /// <summary>The gear rules from gear.json (decisions row 6; phase 2 spec).</summary>
    public sealed class GearCatalog
    {
        public sealed class SlotDef { public string Id, Main; }
        public sealed class TypeDef { public string Id, Slot, Stat; public double Value; }
        public sealed class RarityDef { public string Id; public int Rank; public double MainPct, MainHaste; public int UnlockAfterChapter; }
        public sealed class SetDef { public string Id; public Dictionary<string, double> Two = new Dictionary<string, double>(), Four = new Dictionary<string, double>(); }

        public readonly List<SlotDef> Slots = new List<SlotDef>();
        public readonly List<TypeDef> Types = new List<TypeDef>();
        public readonly List<RarityDef> Rarities = new List<RarityDef>();
        public readonly List<SetDef> Sets = new List<SetDef>();
        public readonly List<double> DropOdds = new List<double>();
        public readonly List<(int count, double chance)> IdlePieces = new List<(int, double)>();
        public double UpgradePct;
        public int MaxUpgrade;
        public long UpgradeGoldPerRankLevel;

        public static GearCatalog FromJson(string json)
        {
            var n = Node.Of(Json.Parse(json));
            var c = new GearCatalog { UpgradePct = n.Num("upgradePct", 0.05), MaxUpgrade = (int)n.Num("maxUpgrade", 20), UpgradeGoldPerRankLevel = (long)n.Num("upgradeGoldPerRankLevel", 1000) };
            foreach (var s in n.Nodes("slots")) c.Slots.Add(new SlotDef { Id = s.Str("id"), Main = s.Str("main") });
            foreach (var t in n.Nodes("types")) c.Types.Add(new TypeDef { Id = t.Str("id"), Slot = t.Str("slot"), Stat = t.Str("stat"), Value = t.Num("value") });
            int rank = 0;
            foreach (var r in n.Nodes("rarities"))
                c.Rarities.Add(new RarityDef { Id = r.Str("id"), Rank = ++rank, MainPct = r.Num("mainPct"), MainHaste = r.Num("mainHaste"), UnlockAfterChapter = (int)r.Num("unlockAfterChapter") });
            foreach (var s in n.Nodes("sets"))
            {
                var d = new SetDef { Id = s.Str("id") };
                var two = s.Obj("two"); if (two != null) foreach (var k in two.Keys) d.Two[k] = two.Num(k);
                var four = s.Obj("four"); if (four != null) foreach (var k in four.Keys) d.Four[k] = four.Num(k);
                c.Sets.Add(d);
            }
            foreach (var o in n.List("dropOdds")) c.DropOdds.Add((double)o);
            foreach (var p in n.Nodes("idlePieces")) c.IdlePieces.Add(((int)p.Num("count"), p.Num("chance")));
            return c;
        }

        public TypeDef Type(string id) => Types.Find(t => t.Id == id);
        public SlotDef Slot(string id) => Slots.Find(s => s.Id == id);
        public SetDef Set(string id) => Sets.Find(s => s.Id == id);
        public RarityDef Rarity(int rank) => Rarities[System.Math.Clamp(rank, 1, Rarities.Count) - 1];
    }
}
