using System.Collections.Generic;
using Gacha.Core.Battle;

namespace Gacha.Core.Campaign
{
    /// <summary>
    /// The starter team (owner, 2026-10-03): 5 random heroes from the save seed, at least one tank and one healer
    /// (slot order: tank, healer, then three of any role). Seeded, so a save always rolls the same team (hard rule 4).
    /// </summary>
    public static class Starter
    {
        public static List<string> Roll(GameData g, ulong seed)
        {
            var rng = new Rng(seed ^ 0x5717A27E5UL);
            var pool = new List<string>(g.HeroOrder);
            var team = new List<string>();
            foreach (var role in new[] { "tank", "healer" })
            {
                var of = pool.FindAll(h => g.Heroes[h].Role == role);
                var pick = of[rng.Range(0, of.Count)];
                team.Add(pick); pool.Remove(pick);
            }
            while (team.Count < 5)
            {
                var pick = pool[rng.Range(0, pool.Count)];
                team.Add(pick); pool.Remove(pick);
            }
            return team;
        }
    }
}
