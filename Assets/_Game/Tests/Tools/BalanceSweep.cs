using System;
using System.Collections.Generic;
using System.Linq;
using Gacha.Core;
using Gacha.Core.Battle;

namespace Gacha.Tests.Tools
{
    /// <summary>
    /// Kind 5: balance report, not a pass/fail test. Random 5v5 teams, each match played twice with sides swapped.
    /// Run: tools/csharp/test.sh -Main Gacha.Tests.Tools.BalanceSweep   (args: matches, seed)
    /// </summary>
    public static class BalanceSweep
    {
        public static int Main(string[] args)
        {
            int matches = args.Length > 0 ? int.Parse(args[0]) : 2000;
            ulong seed = args.Length > 1 ? ulong.Parse(args[1]) : 1;
            var data = TestData.Game;
            var roster = InvariantTests.Roster;
            var rng = new Rng(seed);
            var games = new Dictionary<string, double>(); var wins = new Dictionary<string, double>();
            var dmg = new Dictionary<string, double>(); var heal = new Dictionary<string, double>();
            int timeouts = 0, played = 0; double time = 0;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int m = 0; m < matches; m++)
            {
                var a = InvariantTests.RandomTeam(rng, roster); var b = InvariantTests.RandomTeam(rng, roster);
                ulong s = rng.NextULong();
                for (int side = 0; side < 2; side++)
                {
                    var (x, y) = side == 0 ? (a, b) : (b, a);
                    var bt = new Battle(data, x, y, s);
                    var r = bt.Run();
                    played++; time += r.Time;
                    if (r.Winner < 0) timeouts++;
                    foreach (var u in bt.Units.Where(u => u.IsHero))
                    {
                        double w = r.Winner < 0 ? 0.5 : (r.Winner == u.Team ? 1 : 0);
                        Add(games, u.Id, 1); Add(wins, u.Id, w);
                    }
                    foreach (var e in r.Log)
                    {
                        if (e.Src < 0) continue;
                        var src = bt.Units[e.Src]; string id = src.IsSummon ? bt.Units[src.Owner].Id : src.Id;
                        if (e.Type == Ev.Damage && bt.Units[e.Dst].Team != src.Team) Add(dmg, id, e.Amount);
                        if (e.Type == Ev.Heal) Add(heal, id, e.Amount);
                    }
                }
            }
            Console.WriteLine($"{played} battles in {sw.Elapsed.TotalSeconds:0.0}s; avg {time / played:0.0}s; timeouts {100.0 * timeouts / played:0.0}%");
            Console.WriteLine();
            Console.WriteLine("| Hero | Race | Role | Win % | Dmg/battle | Heal/battle | Flag |");
            Console.WriteLine("| --- | --- | --- | --- | --- | --- | --- |");
            foreach (var id in roster.Where(h => Get(games, h) > 0).OrderByDescending(h => wins[h] / games[h]))
            {
                var d = data.Heroes[id]; double wr = 100 * wins[id] / games[id];
                string flag = wr > 60 ? "STRONG" : wr < 40 ? "WEAK" : "";
                Console.WriteLine($"| {d.Name} | {d.Core} | {d.Role} | {wr:0.0} | {Get(dmg, id) / games[id]:0} | {Get(heal, id) / games[id]:0} | {flag} |");
            }
            Console.WriteLine();
            foreach (var group in new[] { "core", "role" })
            {
                Console.WriteLine($"| {group} | Win % |"); Console.WriteLine("| --- | --- |");
                foreach (var g in roster.GroupBy(h => group == "core" ? data.Heroes[h].Core : data.Heroes[h].Role).OrderBy(g => g.Key))
                    Console.WriteLine($"| {g.Key} | {100 * g.Sum(h => Get(wins, h)) / Math.Max(1, g.Sum(h => Get(games, h))):0.0} |");
                Console.WriteLine();
            }
            return 0;
        }

        static void Add(Dictionary<string, double> d, string k, double v) => d[k] = Get(d, k) + v;
        static double Get(Dictionary<string, double> d, string k) => d.TryGetValue(k, out var v) ? v : 0;
    }
}
