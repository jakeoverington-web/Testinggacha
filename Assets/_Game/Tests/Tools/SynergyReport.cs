using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gacha.Core;
using Gacha.Core.Battle;
using Gacha.Core.Data;

namespace Gacha.Tests.Tools
{
    /// <summary>
    /// Team synergy and diversity (decisions row 29).
    /// A) Package strength: a balanced team built around one synergy keyword (heroes that apply it + heroes that pay it off)
    ///    vs a random balanced team, same opponent and seed.
    /// B) Top teams: hill-climbing search for the strongest teams against a fixed gauntlet; how diverse are they?
    /// Run: tools/csharp/test.sh -Main Gacha.Tests.Tools.SynergyReport [samplesPerPackage=300] [searchStarts=24]
    /// </summary>
    public static class SynergyReport
    {
        internal static GameData D; internal static List<string> R;
        internal static Dictionary<string, List<string>> Applies = new Dictionary<string, List<string>>(), Payoffs = new Dictionary<string, List<string>>();

        /// <summary>Loads the roster and the keyword lists (applies / payoffs) from heroes.json.</summary>
        internal static List<string> Init()
        {
            D = TestData.Game; R = InvariantTests.Roster; Applies.Clear(); Payoffs.Clear();
            foreach (var h in Node.Of(Json.Parse(System.IO.File.ReadAllText(System.IO.Path.Combine(TestData.Dir, "heroes.json")))).Nodes("heroes"))
            {
                foreach (var k in h.Strs("applies")) { if (!Applies.ContainsKey(k)) Applies[k] = new List<string>(); Applies[k].Add(h.Str("id")); }
                foreach (var k in h.Strs("payoffs")) { if (!Payoffs.ContainsKey(k)) Payoffs[k] = new List<string>(); Payoffs[k].Add(h.Str("id")); }
            }
            return Applies.Keys.Where(k => Payoffs.ContainsKey(k) && Payoffs[k].Count > 0).OrderBy(k => k).ToList();
        }
        static readonly string[][] Patterns = {
            new[] { "tank", "healer", "support", "dps", "dps" }, new[] { "tank", "tank", "healer", "dps", "dps" },
            new[] { "tank", "healer", "dps", "dps", "dps" }, new[] { "tank", "healer", "support", "support", "dps" },
            new[] { "tank", "tank", "healer", "support", "dps" } };

        internal static void Par(int n, Action<int> body)
        {
            int next = -1; var ts = new List<Thread>();
            for (int w = 0; w < Environment.ProcessorCount; w++) { var t = new Thread(() => { int i; while ((i = Interlocked.Increment(ref next)) < n) body(i); }, 64 << 20); t.Start(); ts.Add(t); }
            foreach (var t in ts) t.Join();
        }

        static double Score(List<string> a, List<string> b, ulong seed)
        {
            var r = new Battle(D, new TeamSetup(a.ToArray()), new TeamSetup(b.ToArray()), seed) { KeepLog = false }.Run();
            return r.Winner == 0 ? 1 : r.Winner < 0 ? 0.5 : 0;
        }

        internal static List<string> RandomTeam(Rng rng, string[] pattern = null, ICollection<string> fixedHeroes = null)
        {
            pattern ??= Patterns[rng.Range(0, Patterns.Length)];
            var team = new List<string>(fixedHeroes ?? new string[0]);
            var roles = pattern.ToList();
            foreach (var f in team) roles.Remove(D.Heroes[f].Role);
            foreach (var role in roles) { var pool = R.Where(x => D.Heroes[x].Role == role && !team.Contains(x)).ToList(); team.Add(pool[rng.Range(0, pool.Count)]); }
            return team;
        }

        /// <summary>A balanced team with as many package heroes as the roles allow (at least one applier and one payer).</summary>
        internal static List<string> PackageTeam(Rng rng, string kw)
        {
            var members = Applies[kw].Union(Payoffs[kw]).Distinct().OrderBy(_ => rng.NextULong()).ToList();
            for (int tries = 0; tries < 20; tries++)
            {
                var pattern = Patterns[rng.Range(0, Patterns.Length)].ToList();
                var team = new List<string>();
                var a = Applies[kw].OrderBy(_ => rng.NextULong()).FirstOrDefault(x => pattern.Contains(D.Heroes[x].Role));
                if (a == null) continue;
                team.Add(a); pattern.Remove(D.Heroes[a].Role);
                var p = Payoffs[kw].Where(x => x != a).OrderBy(_ => rng.NextULong()).FirstOrDefault(x => pattern.Contains(D.Heroes[x].Role));
                if (p == null) continue;
                team.Add(p); pattern.Remove(D.Heroes[p].Role);
                foreach (var m in members) if (!team.Contains(m) && pattern.Contains(D.Heroes[m].Role)) { team.Add(m); pattern.Remove(D.Heroes[m].Role); }
                foreach (var role in pattern) { var pool = R.Where(x => D.Heroes[x].Role == role && !team.Contains(x)).ToList(); team.Add(pool[rng.Range(0, pool.Count)]); }
                return team;
            }
            return null;
        }

        static List<string> Keywords(List<string> team) =>
            Applies.Keys.Where(k => team.Any(h => Applies[k].Contains(h)) && team.Any(h => Payoffs.TryGetValue(k, out var p) && p.Contains(h) && !(Applies[k].Contains(h) && team.Count(x => Applies[k].Contains(x) || p.Contains(x)) < 2))).OrderBy(k => k).ToList();

        public static int Main(string[] args)
        {
            int n = args.Length > 0 ? int.Parse(args[0]) : 300;
            int starts = args.Length > 1 ? int.Parse(args[1]) : 24;
            var kws = Init();
            var sw = System.Diagnostics.Stopwatch.StartNew();

            // ---------- A: packages ----------
            var res = new (string kw, double up, double se, int size)[kws.Count];
            Par(kws.Count, i =>
            {
                var rng = new Rng(500UL + (ulong)i); double s = 0, sq = 0; int m = 0; double size = 0;
                for (int k = 0; k < n; k++)
                {
                    var team = PackageTeam(rng, kws[i]); if (team == null) break;
                    var ctl = RandomTeam(rng); var opp = RandomTeam(rng); ulong seed = rng.NextULong();
                    double d = Score(team, opp, seed) - Score(ctl, opp, seed);
                    s += d; sq += d * d; m++;
                    size += team.Count(x => Applies[kws[i]].Contains(x) || Payoffs[kws[i]].Contains(x));
                }
                double mean = m == 0 ? 0 : s / m;
                res[i] = (kws[i], 100 * mean, m == 0 ? 0 : 100 * Math.Sqrt(Math.Max(0, sq / m - mean * mean) / m), m == 0 ? 0 : (int)Math.Round(size / m));
            });
            Console.WriteLine($"_Packages: {n} paired battles each. Uplift = points a team built around the keyword gains over a random balanced team (same opponent, same seed)._");
            Console.WriteLine();
            Console.WriteLine("## A. Synergy packages");
            Console.WriteLine();
            Console.WriteLine("| Keyword | Uplift vs random team | ± | Package heroes in team (avg) | Appliers / payers in roster |");
            Console.WriteLine("| --- | --- | --- | --- | --- |");
            foreach (var r in res.OrderByDescending(r => r.up))
                Console.WriteLine($"| {r.kw} | {r.up:+0;-0;0} | {r.se:0} | {r.size} | {Applies[r.kw].Count} / {Payoffs[r.kw].Count} |");
            Console.WriteLine();

            // ---------- B: top teams ----------
            var grng = new Rng(4242);
            var gauntlet = Enumerable.Range(0, 120).Select(_ => (team: RandomTeam(grng), seed: grng.NextULong())).ToList();
            Func<List<string>, double> eval = team => gauntlet.Average(g => Score(team, g.team, g.seed));
            var found = new List<(List<string> team, double wr)>(); var lk = new object();
            Par(starts, i =>
            {
                var rng = new Rng(900UL + (ulong)i);
                var team = i % 2 == 0 ? RandomTeam(rng) : (PackageTeam(rng, kws[rng.Range(0, kws.Count)]) ?? RandomTeam(rng));
                double best = eval(team);
                for (int it = 0; it < 30; it++)
                {
                    int slot = rng.Range(0, 5); string role = D.Heroes[team[slot]].Role;
                    var pool = R.Where(x => D.Heroes[x].Role == role && !team.Contains(x)).ToList();
                    var cand = new List<string>(team); cand[slot] = pool[rng.Range(0, pool.Count)];
                    double v = eval(cand);
                    if (v > best) { best = v; team = cand; }
                }
                lock (lk) found.Add((team, best));
            });
            var top = found.GroupBy(f => string.Join(",", f.team.OrderBy(x => x))).Select(g => g.First()).OrderByDescending(f => f.wr).ToList();
            Console.WriteLine($"## B. Strongest teams found ({starts} hill-climbing searches, 30 swaps each, scored against the same 120 random balanced opponents)");
            Console.WriteLine();
            Console.WriteLine("| Win % | Team | Races | Active synergy keywords |");
            Console.WriteLine("| --- | --- | --- | --- |");
            foreach (var t in top.Take(15))
                Console.WriteLine($"| {100 * t.wr:0} | {string.Join(", ", t.team.OrderBy(x => D.Heroes[x].Role).Select(x => D.Heroes[x].Name))} | {string.Join(" ", t.team.GroupBy(x => D.Heroes[x].Core).OrderByDescending(g => g.Count()).Select(g => g.Count() + " " + g.Key))} | {string.Join(", ", Keywords(t.team))} |");
            Console.WriteLine();
            var top10 = top.Take(10).ToList();
            var usage = top10.SelectMany(t => t.team).GroupBy(x => x).OrderByDescending(g => g.Count()).ToList();
            Console.WriteLine($"Gauntlet top 10: synergies per team {string.Join("/", top10.Select(t => Keywords(t.team).Count))}; {top10.SelectMany(t => Keywords(t.team)).Distinct().Count()} different packages.");
            Console.WriteLine($"Diversity of the top 10: {usage.Count} different heroes; most used: {string.Join(", ", usage.Take(6).Select(g => $"{D.Heroes[g.Key].Name} {g.Count()}/10"))}.");
            var randomWr = gauntlet.Take(40).Select((g, i) => eval(RandomTeam(new Rng(77UL + (ulong)i)))).Average();
            Console.WriteLine($"A random balanced team wins {100 * randomWr:0}% against the same gauntlet.");
            Console.WriteLine();
            // Round robin among the teams found: strong teams against each other (the random gauntlet saturates near 100%)
            var pool = top.Take(24).Select(t => t.team).ToList(); var rr = new double[pool.Count]; var games = new int[pool.Count];
            var pairs = new List<(int a, int b, ulong seed)>(); var prng = new Rng(777);
            for (int x = 0; x < pool.Count; x++) for (int y = x + 1; y < pool.Count; y++) for (int k = 0; k < 4; k++) pairs.Add((x, y, prng.NextULong()));
            var res2 = new double[pairs.Count * 2];
            Par(pairs.Count, i => { var p = pairs[i]; res2[2 * i] = Score(pool[p.a], pool[p.b], p.seed); res2[2 * i + 1] = 1 - Score(pool[p.b], pool[p.a], p.seed); });
            for (int i = 0; i < pairs.Count; i++) { var p = pairs[i]; double v = (res2[2 * i] + res2[2 * i + 1]) / 2; rr[p.a] += v; rr[p.b] += 1 - v; games[p.a]++; games[p.b]++; }
            var ranked = Enumerable.Range(0, pool.Count).OrderByDescending(i => rr[i] / games[i]).ToList();
            Console.WriteLine($"## C. Round robin among the {pool.Count} strongest teams found (each pair: 4 seeds, both sides)");
            Console.WriteLine();
            Console.WriteLine("| Rank | Round-robin win % | Team | Races | Active synergy keywords |");
            Console.WriteLine("| --- | --- | --- | --- | --- |");
            for (int r = 0; r < ranked.Count; r++)
            {
                var t = pool[ranked[r]];
                Console.WriteLine($"| {r + 1} | {100 * rr[ranked[r]] / games[ranked[r]]:0} | {string.Join(", ", t.OrderBy(x => D.Heroes[x].Role).Select(x => D.Heroes[x].Name))} | {string.Join(" ", t.GroupBy(x => D.Heroes[x].Core).OrderByDescending(g => g.Count()).Select(g => g.Count() + " " + g.Key))} | {string.Join(", ", Keywords(t))} |");
            }
            Console.WriteLine();
            var rrTop = ranked.Take(10).Select(i => pool[i]).ToList();
            var rrUse = rrTop.SelectMany(t => t).GroupBy(x => x).OrderByDescending(g => g.Count()).ToList();
            var rrKw = rrTop.SelectMany(Keywords).Distinct().Count();
            Console.WriteLine($"Round-robin top 10: synergies per team {string.Join("/", rrTop.Select(t => Keywords(t).Count))}; {rrKw} different packages; {rrUse.Count} different heroes; most used: {string.Join(", ", rrUse.Take(6).Select(g => $"{D.Heroes[g.Key].Name} {g.Count()}/10"))}.");
            Console.WriteLine();
            Console.Error.WriteLine($"done in {sw.Elapsed.TotalSeconds:0}s");
            return 0;
        }
    }
}
