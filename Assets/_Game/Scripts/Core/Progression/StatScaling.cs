using System;
using Gacha.Core.Data;

namespace Gacha.Core.Progression
{
    /// <summary>
    /// How level and stars scale a hero's HP, ATK and DEF (progression.json; decisions rows 31, 34).
    /// Level L gives 1 + growth x (L - 1); stars multiply on top. Out-of-range inputs clamp.
    /// </summary>
    public sealed class StatScaling
    {
        public double LevelGrowth = 0.04;
        public int MaxLevel = 200;
        public double[] StarMults = { 1.0 };

        public static StatScaling FromJson(string json)
        {
            var n = Node.Of(Json.Parse(json));
            var s = new StatScaling { LevelGrowth = n.Num("levelGrowth", 0.04), MaxLevel = (int)n.Num("maxLevel", 200) };
            var l = n.List("starMult");
            s.StarMults = new double[l.Count];
            for (int i = 0; i < l.Count; i++) s.StarMults[i] = (double)l[i];
            return s;
        }

        public int MaxStars => StarMults.Length;

        public double LevelMult(int level) => 1 + LevelGrowth * (Math.Clamp(level, 1, MaxLevel) - 1);

        public double StarMult(int stars) => StarMults[Math.Clamp(stars, 1, MaxStars) - 1];

        public double Mult(int level, int stars) => LevelMult(level) * StarMult(stars);
    }
}
