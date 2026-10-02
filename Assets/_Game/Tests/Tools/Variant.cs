using System;
using System.Collections.Generic;
using System.Linq;
using Gacha.Core;
using Gacha.Core.Battle;

namespace Gacha.Tests.Tools
{
    /// <summary>
    /// What-if check: the same hero with one property changed, in paired battles (same teams, same seed).
    /// Run: tools/csharp/test.sh -Main Gacha.Tests.Tools.Variant solenne ai=nearest [samples=600]
    /// Changes: ai=&lt;enemy target key&gt;, move=&lt;mode&gt;, range=&lt;m&gt;, atk=&lt;x&gt;, hp=&lt;x&gt;
    /// </summary>
    public static class Variant
    {
        public static int Main(string[] args)
        {
            string hero = args[0]; string change = args[1]; int n = args.Length > 2 ? int.Parse(args[2]) : 600;
            var D = TestData.Game; var R = InvariantTests.Roster;
            var alt = Gacha.Core.Battle.GameData.Load(TestData.Dir);
            var d = alt.Heroes[hero]; var kv = change.Split('=');
            switch (kv[0])
            {
                case "ai": d.EnemyAi = kv[1]; break;
                case "move": d.Move = kv[1]; break;
                case "range": d.Stats.Range = double.Parse(kv[1]); break;
                case "atk": d.Stats.Atk *= double.Parse(kv[1]); break;
                case "hp": d.Stats.Hp *= double.Parse(kv[1]); break;
            }
            var rng = new Rng(31337); double sum = 0, sq = 0;
            for (int k = 0; k < n; k++)
            {
                var own = InvariantTests.RandomTeam(rng, R.Where(h => h != hero).ToList()); own.Heroes[0] = hero;
                var opp = InvariantTests.RandomTeam(rng, R);
                ulong s = rng.NextULong();
                double a = Score(new Battle(D, own, opp, s).Run()), b = Score(new Battle(alt, own, opp, s).Run());
                sum += b - a; sq += (b - a) * (b - a);
            }
            double m = sum / n, se = Math.Sqrt(Math.Max(0, sq / n - m * m) / n);
            Console.WriteLine($"{D.Heroes[hero].Name} with {change}: {100 * m:+0.0;-0.0} points (±{100 * se:0.0}) vs as she is, {n} paired battles");
            return 0;
        }
        static double Score(BattleResult r) => r.Winner == 0 ? 1 : r.Winner < 0 ? 0.5 : 0;
    }
}
