using System.Collections.Generic;
using Gacha.Core.Battle;

namespace Gacha.Core.Campaign
{
    /// <summary>
    /// Quick Deploy (owner, 2026-10-03): the 5 highest-power heroes, tanks in the front slots (0-1) first, then the rest
    /// in power order. Ties keep the order the heroes were given (the roster order on screen). Returns 5 slots.
    /// </summary>
    public static class QuickDeploy
    {
        public static List<string> Pick(GameData g, IEnumerable<string> owned, int level, int stars) =>
            Pick(owned, id => TeamPower.Hero(g, id, level, stars), g);

        /// <summary>Same rule with each hero's own power (real levels, stars and gear).</summary>
        public static List<string> Pick(IEnumerable<string> owned, System.Func<string, double> power, GameData g)
        {
            var pool = new List<(string id, double power, int order)>();
            int n = 0;
            foreach (var id in owned) pool.Add((id, power(id), n++));
            pool.Sort((a, b) => a.power != b.power ? b.power.CompareTo(a.power) : a.order.CompareTo(b.order));
            if (pool.Count > CampaignState.MaxTeam) pool.RemoveRange(CampaignState.MaxTeam, pool.Count - CampaignState.MaxTeam);

            var ordered = new List<string>();
            foreach (var p in pool) if (g.Heroes[p.id].Role == "tank" && ordered.Count < 2) ordered.Add(p.id);
            foreach (var p in pool) if (!ordered.Contains(p.id)) ordered.Add(p.id);
            return CampaignState.Slots(ordered);
        }
    }
}
