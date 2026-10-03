using System.Collections.Generic;
using System.Linq;
using Gacha.Core.Battle;
using Gacha.Core.Economy;

namespace Gacha.Core.Gear
{
    /// <summary>One piece of gear: a named type (fixes its slot and extra stat), a set, a rarity rank 1-4 and an upgrade level.</summary>
    public sealed class GearItem
    {
        public int Id;
        public string Type, Set;
        public int Rarity, Upgrade;
        /// <summary>Hero id wearing it, or "" (gear is not hero-locked).</summary>
        public string EquippedOn = "";
    }

    /// <summary>
    /// The player's gear (decisions row 6; phase 2 spec). A hero wears at most one piece per slot; a piece has one owner.
    /// Stats are never rolled: type, set, rarity and upgrade fully determine them.
    /// </summary>
    public sealed class Inventory
    {
        public readonly List<GearItem> Items = new List<GearItem>();
        public int NextId = 1;

        public GearItem Add(string type, string set, int rarity)
        {
            var item = new GearItem { Id = NextId++, Type = type, Set = set, Rarity = rarity };
            Items.Add(item);
            return item;
        }

        public IEnumerable<GearItem> On(string hero) => Items.Where(i => i.EquippedOn == hero);
        public bool IsGeared(string hero) => Items.Any(i => i.EquippedOn == hero);

        public void Equip(GearCatalog c, GearItem item, string hero)
        {
            string slot = c.Type(item.Type).Slot;
            foreach (var other in On(hero).ToList()) if (c.Type(other.Type).Slot == slot) other.EquippedOn = "";
            item.EquippedOn = hero;
        }

        public void Unequip(GearItem item) => item.EquippedOn = "";

        public long UpgradeCost(GearCatalog c, GearItem item) => c.UpgradeGoldPerRankLevel * item.Rarity * (item.Upgrade + 1);

        public bool Upgrade(GearCatalog c, GearItem item, Wallet w)
        {
            if (item.Upgrade >= c.MaxUpgrade) return false;
            long cost = UpgradeCost(c, item);
            if (w.Get("gold") < cost) return false;
            w.Add("gold", -cost);
            item.Upgrade++;
            return true;
        }

        /// <summary>A base value by rarity plus every bit of gold spent upgrading it.</summary>
        public long SalvageValue(GearCatalog c, GearItem item)
        {
            long v = c.UpgradeGoldPerRankLevel * item.Rarity;
            for (int l = 1; l <= item.Upgrade; l++) v += c.UpgradeGoldPerRankLevel * item.Rarity * l;
            return v;
        }

        public void Salvage(GearCatalog c, GearItem item, Wallet w)
        {
            w.Add("gold", SalvageValue(c, item));
            Items.Remove(item);
        }

        /// <summary>Main stat + extra stat of each worn piece, plus 2- and 4-piece set bonuses.</summary>
        public StatBonus BonusFor(GearCatalog c, string hero)
        {
            var b = new StatBonus();
            var sets = new Dictionary<string, int>();
            foreach (var item in On(hero))
            {
                var type = c.Type(item.Type); var slot = c.Slot(type.Slot); var rarity = c.Rarity(item.Rarity);
                double up = 1 + c.UpgradePct * item.Upgrade;
                b.Add(slot.Main, (slot.Main == "haste" ? rarity.MainHaste : rarity.MainPct) * up);
                b.Add(type.Stat, type.Value * item.Rarity / 4.0);
                sets[item.Set] = (sets.TryGetValue(item.Set, out var n) ? n : 0) + 1;
            }
            foreach (var kv in sets)
            {
                var set = c.Set(kv.Key);
                if (set == null) continue;
                if (kv.Value >= 2) b.Add(set.Two);
                if (kv.Value >= 4) b.Add(set.Four);
            }
            return b;
        }

        /// <summary>Fills each slot with the strongest free piece (rarity, then upgrade); never takes gear off another hero.</summary>
        public void EquipBest(GearCatalog c, string hero)
        {
            foreach (var slot in c.Slots)
            {
                var best = Items.Where(i => c.Type(i.Type).Slot == slot.Id && (i.EquippedOn == "" || i.EquippedOn == hero))
                    .OrderByDescending(i => i.Rarity * (1 + c.UpgradePct * i.Upgrade)).ThenBy(i => i.Id).FirstOrDefault();
                if (best != null) Equip(c, best, hero);
            }
        }
    }
}
