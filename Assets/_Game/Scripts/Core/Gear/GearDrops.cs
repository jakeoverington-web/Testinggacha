namespace Gacha.Core.Gear
{
    /// <summary>
    /// Random gear drops through the shared seeded RNG (hard rule 4; owner, 2026-10-03). Slot type and set are uniform;
    /// rarity favours the newest unlocked (gear.json dropOdds, renormalised). Rarities unlock after the ch 5/10/15 bosses.
    /// </summary>
    public static class GearDrops
    {
        public const int StagesPerChapter = 30;

        public static int UnlockedRarities(GearCatalog c, int highestCleared)
        {
            int n = 0;
            foreach (var r in c.Rarities) if (highestCleared >= r.UnlockAfterChapter * StagesPerChapter) n++;
            return n < 1 ? 1 : n;
        }

        public static GearItem Roll(Rng rng, GearCatalog c, int unlocked, Inventory inv)
        {
            var type = c.Types[rng.Range(0, c.Types.Count)];
            var set = c.Sets[rng.Range(0, c.Sets.Count)];
            // dropOdds[0] is the newest unlocked rarity, [1] the one below, and so on.
            double total = 0;
            for (int k = 0; k < unlocked; k++) total += c.DropOdds[k];
            double roll = rng.NextDouble() * total;
            int rarity = unlocked;
            for (int k = 0; k < unlocked; k++)
            {
                roll -= c.DropOdds[k];
                if (roll < 0) { rarity = unlocked - k; break; }
            }
            return inv.Add(type.Id, set.Id, rarity);
        }

        /// <summary>Pieces from collecting the idle chest: roll a full-chest count, keep each with chance hours / 24.</summary>
        public static int IdleCount(Rng rng, GearCatalog c, double hours)
        {
            if (hours <= 0) return 0;
            double roll = rng.NextDouble(); int count = 0;
            foreach (var (n, chance) in c.IdlePieces) { roll -= chance; if (roll < 0) { count = n; break; } }
            if (count == 0) count = c.IdlePieces[c.IdlePieces.Count - 1].count;
            double keep = System.Math.Min(1, hours / 24.0);
            int kept = 0;
            for (int i = 0; i < count; i++) if (rng.Chance(keep)) kept++;
            return kept;
        }
    }
}
