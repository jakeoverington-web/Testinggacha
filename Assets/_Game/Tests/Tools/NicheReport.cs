using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gacha.Core;
using Gacha.Core.Battle;

namespace Gacha.Tests.Tools
{
    /// <summary>
    /// Niche report (balance kind 5): "every hero is the best pick somewhere; no hero is the best pick everywhere".
    /// Uplift = how much a hero raises her team's result versus a random same-role replacement, same opponent, same seed.
    /// Measured in 13 situations, plus role yardsticks (damage share, clutch healing, damage soaked, effects applied).
    /// Run: tools/csharp/test.sh -Main Gacha.Tests.Tools.NicheReport [samplesPerSituation=100] [seed=1]
    /// </summary>
    public static class NicheReport
    {
        static readonly string[][] Patterns = {
            new[] { "tank", "healer", "support", "dps", "dps" }, new[] { "tank", "tank", "healer", "dps", "dps" },
            new[] { "tank", "healer", "dps", "dps", "dps" }, new[] { "tank", "healer", "support", "support", "dps" },
            new[] { "tank", "tank", "healer", "support", "dps" } };
        static readonly Dictionary<string, string[]> Styles = new Dictionary<string, string[]> {
            ["vs tanky"] = new[] { "tank", "tank", "tank", "healer", "dps" },
            ["vs sustain"] = new[] { "tank", "healer", "healer", "support", "dps" },
            ["vs burst"] = new[] { "tank", "dps", "dps", "dps", "support" },
            ["vs control"] = new[] { "tank", "support", "support", "support", "dps" } };
        static readonly string[] Races = { "high", "dark", "nature", "ocean", "arcane" };
        public static readonly string[] Situations = {
            "random", "vs high", "vs dark", "vs nature", "vs ocean", "vs arcane", "vs tanky", "vs sustain", "vs burst", "vs control", "with partner", "full package", "own race" };

        sealed class Acc
        {
            public double[] Up = new double[Situations.Length], Sq = new double[Situations.Length]; public int[] N = new int[Situations.Length];
            public double Battles, Wins, Dmg, TeamDmg, Heal, Clutch, Taken, TeamTaken, Alive, Effects;
        }

        static GameData D;
        static List<string> Roster;

        public static int Main(string[] args)
        {
            int n = args.Length > 0 ? int.Parse(args[0]) : 100;
            ulong seed = args.Length > 1 ? ulong.Parse(args[1]) : 1;
            D = TestData.Game; Roster = InvariantTests.Roster;
            var acc = Roster.ToDictionary(h => h, h => new Acc());
            var sw = System.Diagnostics.Stopwatch.StartNew();
            int threads = Math.Max(1, Environment.ProcessorCount);
            var workers = new List<Thread>();
            for (int w = 0; w < threads; w++)
            {
                int wi = w;
                var th = new Thread(() =>
                {
                    for (int i = wi; i < Roster.Count; i += threads)
                    {
                        var h = Roster[i];
                        Measure(h, acc[h], n, new Rng(seed * 1000003UL + (ulong)i));
                    }
                }, 64 * 1024 * 1024);
                th.Start(); workers.Add(th);
            }
            foreach (var th in workers) th.Join();
            Print(acc, n, sw.Elapsed.TotalSeconds);
            return 0;
        }

        // ---------------------------------------------------------------- sampling

        static List<string> Pool(string role, string race = null, string melee = null) =>
            Roster.Where(h => D.Heroes[h].Role == role && (race == null || D.Heroes[h].Core == race)
                && (melee == null || D.Heroes[h].Range == melee)).ToList();

        static string PickFrom(Rng rng, List<string> pool, ICollection<string> taken)
        {
            var free = pool.Where(x => !taken.Contains(x)).ToList();
            return free.Count == 0 ? null : free[rng.Range(0, free.Count)];
        }

        static List<string> Fill(Rng rng, string[] pattern, string race, List<string> fixedHeroes)
        {
            var team = new List<string>(fixedHeroes);
            var roles = pattern.ToList();
            foreach (var f in fixedHeroes) roles.Remove(D.Heroes[f].Role);
            foreach (var r in roles)
            {
                var pick = PickFrom(rng, Pool(r, race), team) ?? PickFrom(rng, Pool(r), team);
                team.Add(pick);
            }
            return team;
        }

        static string[] PatternWith(Rng rng, params string[] roles)
        {
            var ok = Patterns.Where(p => roles.GroupBy(x => x).All(g => p.Count(r => r == g.Key) >= g.Count())).ToList();
            return ok.Count > 0 ? ok[rng.Range(0, ok.Count)] : Patterns[0];
        }

        static void Measure(string h, Acc a, int n, Rng rng)
        {
            var def = D.Heroes[h];
            var partners = Partners(h);
            for (int s = 0; s < Situations.Length; s++)
            {
                string sit = Situations[s];
                for (int k = 0; k < n; k++)
                {
                    // our team with the hero
                    List<string> own;
                    if (sit == "with partner" && partners.Count > 0)
                    {
                        var p = partners[rng.Range(0, partners.Count)];
                        own = Fill(rng, PatternWith(rng, def.Role, D.Heroes[p].Role), null, new List<string> { h, p });
                    }
                    else if (sit == "full package" && partners.Count > 0)
                    {
                        // as many of her synergy partners as fit (up to 3), at least one tank or healer kept if the pattern allows
                        var pick = new List<string> { h };
                        foreach (var p in partners.OrderBy(_ => rng.NextULong()))
                            if (pick.Count < 4 && !pick.Contains(p)) pick.Add(p);
                        var roles = pick.Select(x => D.Heroes[x].Role).ToArray();
                        var pattern = Patterns.FirstOrDefault(pt => roles.GroupBy(x => x).All(g => pt.Count(r => r == g.Key) >= g.Count()));
                        if (pattern == null) pattern = roles.Concat(new[] { "tank" }).Take(5).ToArray().Length == 5 ? roles.Concat(new[] { "tank" }).ToArray() : roles.Concat(Enumerable.Repeat("tank", 5 - roles.Length)).ToArray();
                        own = Fill(rng, pattern, null, pick);
                    }
                    else if (sit == "own race") own = Fill(rng, PatternWith(rng, def.Role), def.Core, new List<string> { h });
                    else own = Fill(rng, PatternWith(rng, def.Role), null, new List<string> { h });

                    // the opponent for this situation
                    List<string> opp;
                    if (sit.StartsWith("vs ") && Races.Contains(sit.Substring(3))) opp = Fill(rng, Patterns[rng.Range(0, Patterns.Length)], sit.Substring(3), new List<string>());
                    else if (Styles.TryGetValue(sit, out var style)) opp = Fill(rng, style, null, new List<string>());
                    else opp = Fill(rng, Patterns[rng.Range(0, Patterns.Length)], null, new List<string>());
                    opp = opp.Select(x => x).ToList();

                    // the same team with a random same-role replacement
                    var taken = new HashSet<string>(own);
                    string repl = sit == "own race" ? PickFrom(rng, Pool(def.Role, def.Core), taken) ?? PickFrom(rng, Pool(def.Role), taken)
                                                    : PickFrom(rng, Pool(def.Role), taken);
                    var alt = own.Select(x => x == h ? repl : x).ToList();

                    ulong bs = rng.NextULong();
                    double withH = Play(own, opp, bs, h, a);
                    double without = Play(alt, opp, bs, null, null);
                    a.Up[s] += withH - without; a.Sq[s] += (withH - without) * (withH - without); a.N[s]++;
                }
            }
        }

        static List<string> Partners(string h)
        {
            // heroes listed as synergy partners in heroes.json (via the build's keyword graph)
            var node = Gacha.Core.Data.Node.Of(Gacha.Core.Data.Json.Parse(System.IO.File.ReadAllText(System.IO.Path.Combine(TestData.Dir, "heroes.json"))));
            lock (PartnerCache)
            {
                if (PartnerCache.Count == 0)
                    foreach (var x in node.Nodes("heroes")) PartnerCache[x.Str("id")] = x.Nodes("synergy").Select(s => s.Str("hero")).ToList();
                return PartnerCache.TryGetValue(h, out var l) ? l : new List<string>();
            }
        }
        static readonly Dictionary<string, List<string>> PartnerCache = new Dictionary<string, List<string>>();

        /// <summary>Plays one battle with our team as side A; returns 1 win, 0.5 timeout, 0 loss. Records yardsticks for `track`.</summary>
        static double Play(List<string> own, List<string> opp, ulong seed, string track, Acc a)
        {
            var b = new Battle(D, new TeamSetup(own.ToArray()), new TeamSetup(opp.ToArray()), seed) { KeepLog = false };
            int me = track == null ? -1 : own.IndexOf(track);
            if (me >= 0)
            {
                b.OnEvent = e =>
                {
                    if (e.Src < 0 && e.Type != Ev.Damage) return;
                    int owner = e.Src >= 0 && b.Units[e.Src].IsSummon ? b.Units[e.Src].Owner : e.Src;
                    switch (e.Type)
                    {
                        case Ev.Damage:
                            if (e.Dst >= 0 && b.Units[e.Dst].Team == 1 && owner >= 0 && b.Units[owner].Team == 0) { a.TeamDmg += e.Amount; if (owner == me) a.Dmg += e.Amount; }
                            if (e.Dst >= 0 && b.Units[e.Dst].Team == 0 && b.Units[e.Dst].IsHero && e.What != "spend_hp") { a.TeamTaken += e.Amount; if (e.Dst == me) a.Taken += e.Amount; }
                            break;
                        case Ev.Heal:
                            if (owner == me && e.Amount > 0)
                            {
                                a.Heal += e.Amount;
                                var t = b.Units[e.Dst];
                                if ((t.Hp - e.Amount) / t.MaxHp < 0.3) a.Clutch += e.Amount;
                            }
                            break;
                        case Ev.StatusOn: if (owner == me) a.Effects++; break;
                        case Ev.ShieldGain: if (owner == me) a.Effects++; break;
                    }
                };
            }
            var r = b.Run();
            double score = r.Winner == 0 ? 1 : r.Winner < 0 ? 0.5 : 0;
            if (me >= 0) { a.Battles++; a.Wins += score; if (b.Units[me].Alive) a.Alive++; }
            return score;
        }

        // ---------------------------------------------------------------- report

        static void Print(Dictionary<string, Acc> acc, int n, double secs)
        {
            double battles = acc.Values.Sum(x => x.N.Sum()) * 2;
            double cellSe = acc.Values.SelectMany(a => Enumerable.Range(0, Situations.Length).Select(i =>
            {
                double m = a.Up[i] / Math.Max(1, a.N[i]); double v = a.Sq[i] / Math.Max(1, a.N[i]) - m * m;
                return 100 * Math.Sqrt(Math.Max(0, v) / Math.Max(1, a.N[i]));
            })).Average();
            Console.WriteLine($"_{battles:0} battles in {secs:0}s; {n} paired samples per hero per situation. Noise (one standard error): about ±{cellSe:0} points per cell, ±{cellSe / Math.Sqrt(Situations.Length):0.0} for a hero's overall uplift._");
            Console.WriteLine();
            var rows = new List<(string id, double overall, int best, int worst, int positive, string verdict)>();
            foreach (var h in Roster)
            {
                var a = acc[h];
                var up = Enumerable.Range(0, Situations.Length).Select(i => a.N[i] == 0 ? 0 : 100 * a.Up[i] / a.N[i]).ToArray();
                double overall = 100 * a.Up.Sum() / Math.Max(1, a.N.Sum());
                int best = Array.IndexOf(up, up.Max()), worst = Array.IndexOf(up, up.Min());
                int positive = up.Count(x => x > 2);
                string verdict = overall >= 10 && positive >= 11 ? "DOMINANT" : up.Max() <= 2 ? "NO NICHE" : "niche";
                rows.Add((h, overall, best, worst, positive, verdict));
            }

            Console.WriteLine("## Flags");
            Console.WriteLine();
            var flagged = rows.Where(r => r.verdict != "niche").ToList();
            if (flagged.Count == 0) Console.WriteLine("No hero is dominant and none lacks a niche.");
            foreach (var r in flagged.OrderBy(r => r.verdict).ThenByDescending(r => r.overall))
            {
                var d = D.Heroes[r.id];
                Console.WriteLine($"- **{d.Name}** ({d.Core} {d.Role}): {r.verdict}. Overall {r.overall:+0;-0;0} points; above her peers in {r.positive} of 13 situations; best {Situations[r.best]}, worst {Situations[r.worst]}.");
            }
            Console.WriteLine();

            foreach (var role in new[] { "tank", "dps", "healer", "support" })
            {
                Console.WriteLine($"## {char.ToUpper(role[0]) + role.Substring(1)}s");
                Console.WriteLine();
                string yard = role == "dps" ? "Team damage share" : role == "healer" ? "Heal / clutch heal per battle" : role == "tank" ? "Damage soaked share / survives" : "Effects applied per battle";
                Console.WriteLine($"| Hero | Race | Uplift overall | Best situation | Worst situation | {yard} | Verdict |");
                Console.WriteLine("| --- | --- | --- | --- | --- | --- | --- |");
                foreach (var r in rows.Where(r => D.Heroes[r.id].Role == role).OrderByDescending(r => r.overall))
                {
                    var d = D.Heroes[r.id]; var a = acc[r.id];
                    var up = Enumerable.Range(0, Situations.Length).Select(i => 100 * a.Up[i] / Math.Max(1, a.N[i])).ToArray();
                    string y = role == "dps" ? $"{100 * a.Dmg / Math.Max(1, a.TeamDmg):0}%"
                             : role == "healer" ? $"{a.Heal / a.Battles:0} / {a.Clutch / a.Battles:0}"
                             : role == "tank" ? $"{100 * a.Taken / Math.Max(1, a.TeamTaken):0}% / {100 * a.Alive / a.Battles:0}%"
                             : $"{a.Effects / a.Battles:0.0}";
                    Console.WriteLine($"| {d.Name} | {d.Core} | {r.overall:+0;-0;0} | {Situations[r.best]} ({up[r.best]:+0;-0;0}) | {Situations[r.worst]} ({up[r.worst]:+0;-0;0}) | {y} | {r.verdict} |");
                }
                Console.WriteLine();
            }

            Console.WriteLine("## Uplift by situation (points versus a random same-role replacement)");
            Console.WriteLine();
            Console.WriteLine("| Hero | " + string.Join(" | ", Situations) + " |");
            Console.WriteLine("| --- |" + string.Concat(Enumerable.Repeat(" --- |", Situations.Length)));
            foreach (var h in Roster.OrderBy(h => D.Heroes[h].Core).ThenBy(h => D.Heroes[h].Role))
            {
                var a = acc[h];
                Console.WriteLine($"| {D.Heroes[h].Name} | " + string.Join(" | ", Enumerable.Range(0, Situations.Length).Select(i => $"{100 * a.Up[i] / Math.Max(1, a.N[i]):+0;-0;0}")) + " |");
            }
            Console.WriteLine();

            Console.WriteLine("## Races");
            Console.WriteLine();
            Console.WriteLine("| Race | Win % of its heroes in single-race teams | Uplift of its heroes vs each race (high / dark / nature / ocean / arcane) |");
            Console.WriteLine("| --- | --- | --- |");
            foreach (var race in Races)
            {
                var hs = Roster.Where(h => D.Heroes[h].Core == race).ToList();
                int own = Array.IndexOf(Situations, "own race");
                string vs = string.Join(" / ", Races.Select(r2 => { int si = Array.IndexOf(Situations, "vs " + r2); return $"{hs.Average(h => 100 * acc[h].Up[si] / Math.Max(1, acc[h].N[si])):+0;-0;0}"; }));
                Console.WriteLine($"| {race} | {hs.Average(h => 100 * acc[h].Up[own] / Math.Max(1, acc[h].N[own])):+0;-0;0} uplift | {vs} |");
            }
        }
    }
}
