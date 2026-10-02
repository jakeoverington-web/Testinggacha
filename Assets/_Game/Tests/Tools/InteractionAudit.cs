using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gacha.Core;
using Gacha.Core.Battle;

namespace Gacha.Tests.Tools
{
    /// <summary>
    /// Looks for interactions that multiply unexpectedly.
    /// A) every pair of heroes on one team vs the dummy line: team damage compared with the two heroes apart;
    /// B) the biggest single hits in real battles, with the multiplier stack that built them, plus loop checks;
    /// C) the expected ceiling: one damage dealer with the best four helpers (greedy).
    /// Run: tools/csharp/test.sh -Main Gacha.Tests.Tools.InteractionAudit [realBattles=3000]
    /// </summary>
    public static class InteractionAudit
    {
        static GameData D;
        static List<string> R;

        sealed class Line { public Dictionary<string, double> By = new Dictionary<string, double>(); public double Dmg, MaxAmp; public string MaxAmpWhat; public Dictionary<string, int> Ults = new Dictionary<string, int>(); public Dictionary<string, int> Casts = new Dictionary<string, int>(); }

        /// <summary>30 s vs four 3,600-HP DEF 120 dummies (refilled every tick) and a striker that hits back. Team = heroes + idle ally dummies.</summary>
        static Line DummyLine(List<string> heroes, ulong seed)
        {
            var team = new List<string>(heroes); while (team.Count < 5) team.Add("ally_dummy");
            var b = new Battle(D, new TeamSetup(team.ToArray()), new TeamSetup("target_dummy", "target_dummy", "target_dummy", "target_dummy", "striker"), seed) { KeepLog = false };
            foreach (var e in b.Units.Where(u => u.Team == 1)) { e.Base.Def = 120; if (e.Id == "target_dummy") { e.Base.Hp = 3600; e.Hp = 3600; } }
            var striker = b.Units.Last(u => u.Team == 1); striker.Base.Atk = 60; striker.Untargetable = true;
            var line = new Line();
            b.OnEvent = e =>
            {
                if (e.Src < 0) return;
                var s = b.Units[e.Src]; int owner = s.IsSummon ? s.Owner : s.Index;
                if (b.Units[owner].Team != 0) return;
                if (e.Type == Ev.Damage && e.Dst >= 0 && b.Units[e.Dst].Team == 1) { line.Dmg += e.Amount; var oid = b.Units[owner].Id; line.By[oid] = (line.By.TryGetValue(oid, out var dd) ? dd : 0) + e.Amount; }
                if (e.Type == Ev.Cast) { var d = e.What == "ult" ? line.Ults : line.Casts; var id = b.Units[owner].Id; d[id] = (d.TryGetValue(id, out var c) ? c : 0) + 1; }
            };
            b.OnHit = tr =>
            {
                if (b.Units[tr.Src].Team == 0 && tr.Amplification > line.MaxAmp) { line.MaxAmp = tr.Amplification; line.MaxAmpWhat = $"{b.Units[tr.Src].Id} {tr.Ability}: {tr}"; }
            };
            b.AfterTick = x =>
            {
                foreach (var u in x.Units.Where(u => u.Team == 1 && u.Id == "target_dummy")) { if (!u.Alive) { u.Alive = true; u.Revived = false; } u.Hp = u.MaxHp; }
                foreach (var u in x.Units.Where(u => u.Team == 0 && u.IsHero && u.Alive && u.Hp < u.MaxHp * 0.3)) u.Hp = u.MaxHp * 0.3;   // keep the team in the lab
            };
            b.RunFor(30);
            return line;
        }

        static void Parallel(int n, Action<int> body)
        {
            int next = -1; var ts = new List<Thread>();
            for (int w = 0; w < Environment.ProcessorCount; w++)
            {
                var t = new Thread(() => { int i; while ((i = Interlocked.Increment(ref next)) < n) body(i); }, 64 << 20);
                t.Start(); ts.Add(t);
            }
            foreach (var t in ts) t.Join();
        }

        public static int Main(string[] args)
        {
            int realBattles = args.Length > 0 ? int.Parse(args[0]) : 3000;
            D = TestData.Game; R = InvariantTests.Roster;
            var sw = System.Diagnostics.Stopwatch.StartNew();

            // ---------- A: pairs ----------
            var solo = new Dictionary<string, Line>();
            var soloArr = new Line[R.Count];
            Parallel(R.Count, i => soloArr[i] = Avg(new List<string> { R[i] }));
            for (int i = 0; i < R.Count; i++) solo[R[i]] = soloArr[i];
            var pairs = new List<(string a, string b)>();
            for (int i = 0; i < R.Count; i++) for (int j = i + 1; j < R.Count; j++) pairs.Add((R[i], R[j]));
            var pairLines = new Line[pairs.Count];
            Parallel(pairs.Count, k => pairLines[k] = Avg(new List<string> { pairs[k].a, pairs[k].b }));

            Console.WriteLine($"_Pairs: {pairs.Count} x 2 seeds vs the dummy line; real battles: {realBattles}._"); Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("## A. Partners that multiply a hero's own damage (her damage with the partner ÷ her damage alone)");
            Console.WriteLine();
            Console.WriteLine("Only heroes who deal at least 3,000 damage alone in 30 s, so idle-alone artifacts are excluded.");
            Console.WriteLine();
            Console.WriteLine("| Hero | Partner | Her damage alone | With partner | Multiplier | Biggest hit stack in the pair |");
            Console.WriteLine("| --- | --- | --- | --- | --- | --- |");
            var ordered = new List<(string a, string b, double alone, double with, double mult, string stack)>();
            for (int k = 0; k < pairs.Count; k++)
            {
                var (pa, pb) = pairs[k]; var l = pairLines[k];
                foreach (var (x, y) in new[] { (pa, pb), (pb, pa) })
                {
                    double alone = solo[x].By.TryGetValue(x, out var s1) ? s1 : 0;
                    if (alone < 3000) continue;
                    double with = l.By.TryGetValue(x, out var s2) ? s2 : 0;
                    ordered.Add((x, y, alone, with, with / alone, l.MaxAmpWhat));
                }
            }
            var ranked = ordered.OrderByDescending(x => x.mult).ToList();
            foreach (var x in ranked.Take(20))
                Console.WriteLine($"| {Name(x.a)} | {Name(x.b)} | {x.alone:0} | {x.with:0} | x{x.mult:0.00} | {x.stack} |");
            Console.WriteLine();
            Console.WriteLine($"Median partner multiplier x{Median(ranked.Select(x => x.mult)):0.00}; above x1.5: {ranked.Count(x => x.mult > 1.5)} of {ranked.Count}; above x2: {ranked.Count(x => x.mult > 2.0)}.");
            Console.WriteLine();
            Console.WriteLine("Loop check (ultimates in 30 s, solo → best pair):");
            Console.WriteLine();
            var loops = new List<string>();
            foreach (var h in R)
            {
                int s = solo[h].Ults.TryGetValue(h, out var su) ? su : 0;
                var best = pairs.Select((p, k) => (p, line: pairLines[k])).Where(x => x.p.a == h || x.p.b == h).Select(x => (x.p, u: x.line.Ults.TryGetValue(h, out var pu) ? pu : 0)).OrderByDescending(x => x.u).First();
                if (best.u >= Math.Max(4, s * 2)) loops.Add($"- {Name(h)}: {s} alone → {best.u} with {Name(best.p.a == h ? best.p.b : best.p.a)}");
            }
            Console.WriteLine(loops.Count == 0 ? "- No hero doubles her ultimates from any single partner." : string.Join("\n", loops));
            Console.WriteLine();

            // ---------- B: biggest hits in real battles ----------
            var top = new List<(double amp, string what)>(); var lockTop = new object();
            var ampByHero = new Dictionary<string, List<double>>();
            int maxUlts = 0; string maxUltsWho = ""; int maxEventsTick = 0; string maxEventsWho = "";
            var rngs = Enumerable.Range(0, realBattles).Select(i => new Rng(9000UL + (ulong)i)).ToArray();
            Parallel(realBattles, i =>
            {
                var rng = rngs[i];
                var b = new Battle(D, InvariantTests.RandomTeam(rng, R), InvariantTests.RandomTeam(rng, R), rng.NextULong()) { KeepLog = false };
                var local = new List<(double, string)>(); var localAmp = new Dictionary<string, List<double>>();
                var ults = new Dictionary<int, int>(); int evTick = 0, evMax = 0; double tick = -1;
                b.OnHit = tr =>
                {
                    var s = b.Units[tr.Src]; string id = s.IsSummon ? s.Id : s.Id;
                    if (!localAmp.TryGetValue(id, out var l)) localAmp[id] = l = new List<double>();
                    l.Add(tr.Amplification);
                    if (tr.Amplification > 3.0)
                    {
                        var t = b.Units[tr.Dst];
                        string buffs = string.Join(",", s.Statuses.Where(x => Battle.Buffs.Contains(x.Id)).Select(x => x.Id).Distinct());
                        string debuffs = string.Join(",", t.Statuses.Where(x => Battle.Debuffs.Contains(x.Id)).Select(x => x.Id).Distinct());
                        local.Add((tr.Amplification, $"{s.Name} {tr.Ability} ({tr.Kind}) → {t.Name}: {tr}. Attacker buffs [{buffs}], target debuffs [{debuffs}]"));
                    }
                };
                b.OnEvent = e =>
                {
                    if (b.Time != tick) { tick = b.Time; evTick = 0; }
                    if (++evTick > evMax) evMax = evTick;
                    if (e.Type == Ev.Cast && e.What == "ult") ults[e.Src] = (ults.TryGetValue(e.Src, out var c) ? c : 0) + 1;
                };
                b.Run();
                lock (lockTop)
                {
                    top.AddRange(local); if (top.Count > 400) top = top.OrderByDescending(x => x.amp).Take(200).ToList();
                    foreach (var kv in localAmp) { if (!ampByHero.TryGetValue(kv.Key, out var l)) ampByHero[kv.Key] = l = new List<double>(); l.AddRange(kv.Value); }
                    foreach (var kv in ults) if (kv.Value > maxUlts) { maxUlts = kv.Value; maxUltsWho = b.Units[kv.Key].Name; }
                    if (evMax > maxEventsTick) { maxEventsTick = evMax; maxEventsWho = string.Join(", ", b.Units.Where(u => u.IsHero).Select(u => u.Name)); }
                }
            });
            Console.WriteLine("## B. Biggest hit stacks in real battles");
            Console.WriteLine();
            Console.WriteLine("Amplification = everything stacked on top of the ability's own % at base ATK (ATK buffs × passives × race × crit × damage taken × vs shield), before DEF and block.");
            Console.WriteLine();
            Console.WriteLine("| Hero | Hits | Typical (median) | 99th percentile | Max |");
            Console.WriteLine("| --- | --- | --- | --- | --- |");
            foreach (var kv in ampByHero.Where(kv => kv.Value.Count > 50).OrderByDescending(kv => Pct(kv.Value, 0.99)).Take(15))
                Console.WriteLine($"| {kv.Key} | {kv.Value.Count} | x{Pct(kv.Value, 0.5):0.00} | x{Pct(kv.Value, 0.99):0.00} | x{kv.Value.Max():0.00} |");
            Console.WriteLine();
            Console.WriteLine("Top 10 single hits:");
            Console.WriteLine();
            foreach (var x in top.OrderByDescending(x => x.amp).Take(10)) Console.WriteLine($"- {x.what}");
            Console.WriteLine();
            Console.WriteLine($"Most ultimates by one hero in one battle: {maxUlts} ({maxUltsWho}). Most events in one 0.1 s tick: {maxEventsTick}.");
            Console.WriteLine();

            // ---------- C: one damage dealer with the best four helpers ----------
            Console.WriteLine("## C. Expected ceiling: one damage dealer's own damage with the best four helpers (greedy pick vs the dummy line)");
            Console.WriteLine();
            Console.WriteLine("| Damage dealer | Alone (30 s) | Best four helpers | With them | Multiplier |");
            Console.WriteLine("| --- | --- | --- | --- | --- |");
            var dps = R.Where(h => D.Heroes[h].Role == "dps").ToList();
            var helpers = R.Where(h => D.Heroes[h].Role == "support" || D.Heroes[h].Role == "healer").ToList();
            var ceil = new (string dps, List<string> team, double dmg)[dps.Count];
            Parallel(dps.Count, i =>
            {
                var team = new List<string> { dps[i] };
                for (int step = 0; step < 4; step++)
                {
                    string best = null; double bestDmg = -1;
                    foreach (var h in helpers.Where(h => !team.Contains(h)))
                    {
                        var l = DummyLine(team.Concat(new[] { h }).ToList(), 5);
                        double own = l.By.TryGetValue(dps[i], out var od) ? od : 0;
                        if (own > bestDmg) { bestDmg = own; best = h; }
                    }
                    team.Add(best);
                }
                var fin = Avg(team); ceil[i] = (dps[i], team, fin.By.TryGetValue(dps[i], out var fd) ? fd : 0);
            });
            foreach (var c in ceil.OrderByDescending(c => c.dmg / solo[c.dps].By[c.dps]))
                Console.WriteLine($"| {Name(c.dps)} | {solo[c.dps].By[c.dps]:0} | {string.Join(", ", c.team.Skip(1).Select(Name))} | {c.dmg:0} | x{c.dmg / solo[c.dps].By[c.dps]:0.00} |");
            Console.Error.WriteLine($"done in {sw.Elapsed.TotalSeconds:0}s");
            return 0;
        }

        static Line Avg(List<string> heroes)
        {
            var a = DummyLine(heroes, 1); var b = DummyLine(heroes, 2);
            var r = new Line { Dmg = (a.Dmg + b.Dmg) / 2, MaxAmp = Math.Max(a.MaxAmp, b.MaxAmp), MaxAmpWhat = a.MaxAmp >= b.MaxAmp ? a.MaxAmpWhat : b.MaxAmpWhat };
            foreach (var k in a.By.Keys.Union(b.By.Keys)) r.By[k] = ((a.By.TryGetValue(k, out var x) ? x : 0) + (b.By.TryGetValue(k, out var y) ? y : 0)) / 2;
            foreach (var kv in a.Ults) r.Ults[kv.Key] = kv.Value;
            foreach (var kv in b.Ults) r.Ults[kv.Key] = Math.Max(kv.Value, r.Ults.TryGetValue(kv.Key, out var v) ? v : 0);
            return r;
        }

        static string Name(string id) => D.Heroes[id].Name;
        static double Median(IEnumerable<double> x) => Pct(x.ToList(), 0.5);
        static double Pct(List<double> x, double p) { var s = x.OrderBy(v => v).ToList(); return s.Count == 0 ? 0 : s[Math.Min(s.Count - 1, (int)(p * s.Count))]; }
    }
}
