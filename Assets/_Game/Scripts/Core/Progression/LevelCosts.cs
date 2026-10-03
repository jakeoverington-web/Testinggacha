using System;
using System.Collections.Generic;
using Gacha.Core.Data;

namespace Gacha.Core.Progression
{
    /// <summary>
    /// Level caps per star and the cost of each level (progression.json; phase 2 spec). Xp(L) / Gold(L) = cost to reach L
    /// from L - 1, interpolated log-linearly between the plan doc's breakpoints; Starlight(L) is spent at every 20th level.
    /// </summary>
    public sealed class LevelCosts
    {
        readonly int[] _caps;
        readonly List<(int level, double xp, double gold)> _bp = new List<(int, double, double)>();
        readonly Dictionary<int, int> _starlight = new Dictionary<int, int>();

        LevelCosts(int[] caps) { _caps = caps; }

        public static LevelCosts FromJson(string json)
        {
            var n = Node.Of(Json.Parse(json));
            var capList = n.List("levelCaps");
            var caps = new int[capList.Count];
            for (int i = 0; i < caps.Length; i++) caps[i] = (int)(double)capList[i];
            var lc = new LevelCosts(caps);
            var cost = n.Obj("levelCost");
            foreach (var k in cost.Keys)
            {
                var pair = cost.List(k);
                lc._bp.Add((int.Parse(k), (double)pair[0], (double)pair[1]));
            }
            lc._bp.Sort((a, b) => a.level.CompareTo(b.level));
            var sl = n.Obj("starlight");
            foreach (var k in sl.Keys) lc._starlight[int.Parse(k)] = (int)sl.Num(k);
            return lc;
        }

        public int MaxStars => _caps.Length;

        /// <summary>Top-five level cap for a star rank (clamped to 1..max stars).</summary>
        public int Cap(int stars) => _caps[Math.Clamp(stars, 1, _caps.Length) - 1];

        public double Xp(int level) => Interp(level, xp: true);
        public double Gold(int level) => Interp(level, xp: false);
        public int Starlight(int level) => _starlight.TryGetValue(level, out var v) ? v : 0;

        double Interp(int level, bool xp)
        {
            if (level < _bp[0].level) return 0;
            for (int i = 0; i + 1 < _bp.Count; i++)
            {
                var a = _bp[i]; var b = _bp[i + 1];
                if (level > b.level) continue;
                double t = (level - a.level) / (double)(b.level - a.level);
                double va = xp ? a.xp : a.gold, vb = xp ? b.xp : b.gold;
                return Math.Round(Math.Exp(Math.Log(va) * (1 - t) + Math.Log(vb) * t));
            }
            var last = _bp[_bp.Count - 1];
            return level == last.level ? (xp ? last.xp : last.gold) : 0;
        }
    }

    /// <summary>What reaching each star costs (progression.json "starCosts").</summary>
    public sealed class StarCosts
    {
        public struct Row { public int Copies, Fodder; public long Gold; }
        readonly Dictionary<int, Row> _rows = new Dictionary<int, Row>();

        public static StarCosts FromJson(string json)
        {
            var sc = new StarCosts();
            foreach (var r in Node.Of(Json.Parse(json)).Nodes("starCosts"))
                sc._rows[(int)r.Num("to")] = new Row { Copies = (int)r.Num("copies"), Fodder = (int)r.Num("fodder"), Gold = (long)r.Num("gold") };
            return sc;
        }

        /// <summary>Cost to go from toStars - 1 to toStars.</summary>
        public Row For(int toStars) => _rows.TryGetValue(toStars, out var r) ? r : throw new ArgumentOutOfRangeException(nameof(toStars));
    }
}
