using System.Collections.Generic;
using Gacha.Core.Battle;

namespace Gacha.Core.Campaign
{
    /// <summary>One hero's totals for a fight. Alive is false for a hero who fell; her totals up to then are kept.</summary>
    public sealed class HeroLine
    {
        public string Hero;
        public int Team;
        public double Dealt, Healed, Taken;
        public bool Alive;
    }

    /// <summary>
    /// After-fight statistics (owner, 2026-10-03): damage dealt to enemies, healing done and damage taken per hero, in
    /// field order (setup order = slot order: front row, then back). Damage counts the full hit, so damage soaked by
    /// shields counts as dealt and taken; whole-hit shield blocks (skill and hit shields) count too. Summons count for
    /// their owner. Self-inflicted costs and friendly fire are left out.
    /// </summary>
    public static class FightStats
    {
        static readonly HashSet<string> ShieldBlocks = new HashSet<string> { "skill_shield", "hit_shield" };

        public static List<HeroLine> From(Battle.Battle b)
        {
            var lines = new List<HeroLine>();
            var byUnit = new Dictionary<int, HeroLine>();
            foreach (var u in b.Units)
            {
                if (!u.IsHero) continue;
                var line = new HeroLine { Hero = u.Id, Team = u.Team, Alive = u.Alive };
                lines.Add(line);
                byUnit[u.Index] = line;
            }
            HeroLine Owner(int index)
            {
                if (index < 0 || index >= b.Units.Count) return null;
                var u = b.Units[index];
                if (u.IsSummon) return u.Owner >= 0 && byUnit.TryGetValue(u.Owner, out var o) ? o : null;
                return byUnit.TryGetValue(index, out var l) ? l : null;
            }
            foreach (var e in b.Log)
            {
                bool hit = e.Type == Ev.Damage || (e.Type == Ev.Absorb && ShieldBlocks.Contains(e.What));
                if (hit)
                {
                    if (e.Src == e.Dst || e.Dst < 0) continue;                       // self-inflicted (e.g. spend_hp)
                    var src = Owner(e.Src); var dst = byUnit.TryGetValue(e.Dst, out var d) ? d : null;
                    int srcTeam = e.Src >= 0 ? b.Units[e.Src].Team : -1;
                    if (srcTeam == b.Units[e.Dst].Team) continue;                     // friendly fire
                    if (src != null) src.Dealt += e.Amount;
                    if (dst != null) dst.Taken += e.Amount;
                }
                else if (e.Type == Ev.Heal)
                {
                    var src = Owner(e.Src);
                    if (src != null) src.Healed += e.Amount;
                }
            }
            return lines;
        }
    }
}
