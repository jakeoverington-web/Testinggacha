using System;
using System.Collections.Generic;
using System.Linq;
using Gacha.Core.Campaign;

namespace Gacha.Tests.Tools
{
    /// <summary>
    /// Phase 1c gate: a player who owns the 4 StageGen reference teams plays stages 1 … chapters × 30 in order at the
    /// expected level and stars, through CampaignState.Fight. On each stage they try their teams in turn (the one that
    /// won last first), up to `tries` attempts per team, and swap when one keeps losing (row 18; owner, 2026-10-03).
    /// Run: pwsh -File tools/csharp/test.ps1 -Main Gacha.Tests.Tools.CampaignGate   (args: chapters=5, tries=15, seed=1)
    /// Exit code 0 only if every stage is cleared.
    /// </summary>
    public static class CampaignGate
    {
        public static int Main(string[] args)
        {
            int chapters = args.Length > 0 ? int.Parse(args[0]) : 5;
            int tries = args.Length > 1 ? int.Parse(args[1]) : 15;
            ulong seed = args.Length > 2 ? ulong.Parse(args[2]) : 1;
            var g = TestData.Game;
            int last = Math.Min(chapters * StageGen.PerChapter, g.Stages.Count);
            var refs = StageGen.ReferenceTeams(seed, StageGen.HeroInfo.Load());
            var st = CampaignState.New(seed: 1000 + seed, now: 0);
            var winsBy = new int[refs.Count];
            var swaps = new List<string>();
            int firstTry = 0, current = 0, overFive = 0, maxAttempts = 0;
            for (int i = 1; i <= last; i++)
            {
                int attempts = 0, winner = -1;
                for (int n = 0; n < refs.Count && winner < 0; n++)
                {
                    int t = (current + n) % refs.Count;
                    for (int k = 0; k < tries && winner < 0; k++) { attempts++; if (st.Fight(g, i, refs[t], 0).Won) winner = t; }
                }
                if (winner < 0) break;
                if (attempts == 1) firstTry++;
                if (attempts > 5) overFive++;
                maxAttempts = Math.Max(maxAttempts, attempts);
                if (winner != current) swaps.Add($"{g.Stages[i - 1].Id} → team {winner + 1}");
                current = winner; winsBy[winner]++;
            }
            bool clear = st.HighestCleared >= last;
            Console.WriteLine($"Campaign gate: chapters 1-{chapters} ({last} stages), {refs.Count} reference teams (StageGen seed {seed}), up to {tries} tries per team per stage.");
            Console.WriteLine();
            Console.WriteLine("| Team | Heroes | Stages won |");
            Console.WriteLine("| --- | --- | --- |");
            for (int t = 0; t < refs.Count; t++) Console.WriteLine($"| {t + 1} | {string.Join(", ", refs[t])} | {winsBy[t]} |");
            Console.WriteLine();
            Console.WriteLine($"Cleared: {(clear ? $"all {last}" : $"stuck at {g.Stages[st.HighestCleared].Id}")}. Won on the first try: {firstTry}. Needed more than 5 tries: {overFive} (most: {maxAttempts}). Team swaps: {swaps.Count}{(swaps.Count > 0 ? " (" + string.Join(", ", swaps) + ")" : "")}.");
            Console.WriteLine();
            Console.WriteLine(clear ? "GATE PASSED" : "GATE FAILED");
            return clear ? 0 : 1;
        }
    }
}
