using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gacha.Core;
using Gacha.Core.Battle;

namespace Gacha.Tests.Tools
{
    /// <summary>
    /// What each part of a kit is worth: paired battles with the full hero vs the same hero with one part switched off
    /// (ultimate, skill 1, skill 2, passive, auto attacks). Value = points of team result lost without it.
    /// Run: tools/csharp/test.sh -Main Gacha.Tests.Tools.Ablation [samplesPerHero=300] [seed=1]
    /// </summary>
    public static class Ablation
    {
        public static readonly string[] Parts = { "ult", "s1", "s2", "passive", "basic" };
        static readonly string[][] Patterns = {
            new[] { "tank", "healer", "support", "dps", "dps" }, new[] { "tank", "tank", "healer", "dps", "dps" },
            new[] { "tank", "healer", "dps", "dps", "dps" }, new[] { "tank", "healer", "support", "support", "dps" },
            new[] { "tank", "tank", "healer", "support", "dps" } };

        public static int Main(string[] args)
        {
            int n = args.Length > 0 ? int.Parse(args[0]) : 300;
            ulong seed = args.Length > 1 ? ulong.Parse(args[1]) : 1;
            var D = TestData.Game; var R = InvariantTests.Roster;
            var value = new Dictionary<string, double[]>(); var se = new Dictionary<string, double[]>();
            int next = -1; var lk = new object(); var ts = new List<Thread>();
            for (int w = 0; w < Environment.ProcessorCount; w++)
            {
                var th = new Thread(() =>
                {
                    int i;
                    while ((i = Interlocked.Increment(ref next)) < R.Count)
                    {
                        string h = R[i]; var rng = new Rng(seed * 7919UL + (ulong)i);
                        var sum = new double[Parts.Length]; var sq = new double[Parts.Length];
                        for (int k = 0; k < n; k++)
                        {
                            var own = Fill(D, R, rng, h); var opp = Fill(D, R, rng, null);
                            ulong bs = rng.NextULong();
                            double full = Play(D, own, opp, bs, h, null);
                            for (int p = 0; p < Parts.Length; p++)
                            {
                                double d = full - Play(D, own, opp, bs, h, Parts[p]);
                                sum[p] += d; sq[p] += d * d;
                            }
                        }
                        lock (lk)
                        {
                            value[h] = sum.Select(s => 100 * s / n).ToArray();
                            se[h] = Enumerable.Range(0, Parts.Length).Select(p => { double m = sum[p] / n; return 100 * Math.Sqrt(Math.Max(0, sq[p] / n - m * m) / n); }).ToArray();
                        }
                    }
                }, 64 << 20);
                th.Start(); ts.Add(th);
            }
            foreach (var th in ts) th.Join();

            double noise = se.Values.SelectMany(x => x).Average();
            Console.WriteLine($"_{R.Count * n * (Parts.Length + 1)} battles; {n} paired samples per hero; noise about ±{noise:0.0} points per cell (one standard error)._");
            Console.WriteLine();
            Console.WriteLine("Value = points of team result lost when that part is switched off. **Bold** = more than 2.5x noise away from the role average.");
            foreach (var role in new[] { "tank", "dps", "healer", "support" })
            {
                var hs = R.Where(h => D.Heroes[h].Role == role).ToList();
                var avg = Enumerable.Range(0, Parts.Length).Select(p => hs.Average(h => value[h][p])).ToArray();
                Console.WriteLine();
                Console.WriteLine($"## {char.ToUpper(role[0]) + role.Substring(1)}s");
                Console.WriteLine();
                Console.WriteLine("| Hero | Ultimate | Skill 1 | Skill 2 | Passive | Auto attacks |");
                Console.WriteLine("| --- | --- | --- | --- | --- | --- |");
                Console.WriteLine("| *role average* | " + string.Join(" | ", avg.Select(a => $"*{a:0.0}*")) + " |");
                foreach (var h in hs.OrderByDescending(h => value[h].Sum()))
                    Console.WriteLine($"| {D.Heroes[h].Name} | " + string.Join(" | ", Enumerable.Range(0, Parts.Length).Select(p =>
                    {
                        double v = value[h][p]; bool far = Math.Abs(v - avg[p]) > 2.5 * Math.Max(0.5, se[h][p]);
                        return far ? $"**{v:0.0}**" : $"{v:0.0}";
                    })) + " |");
            }
            return 0;
        }

        static List<string> Fill(GameData D, List<string> R, Rng rng, string hero)
        {
            var ok = Patterns.Where(p => hero == null || p.Contains(D.Heroes[hero].Role)).ToList();
            var pattern = ok[rng.Range(0, ok.Count)].ToList();
            var team = new List<string>();
            if (hero != null) { team.Add(hero); pattern.Remove(D.Heroes[hero].Role); }
            foreach (var role in pattern)
            {
                var pool = R.Where(x => D.Heroes[x].Role == role && !team.Contains(x)).ToList();
                team.Add(pool[rng.Range(0, pool.Count)]);
            }
            return team;
        }

        static double Play(GameData D, List<string> own, List<string> opp, ulong seed, string hero, string off)
        {
            var b = new Battle(D, new TeamSetup(own.ToArray()), new TeamSetup(opp.ToArray()), seed) { KeepLog = false };
            if (off != null) b.Units[own.IndexOf(hero)].Off = new HashSet<string> { off };
            var r = b.Run();
            return r.Winner == 0 ? 1 : r.Winner < 0 ? 0.5 : 0;
        }
    }
}
