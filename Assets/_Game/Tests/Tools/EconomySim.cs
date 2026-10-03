using System;
using Gacha.Core.Campaign;
using Gacha.Core.Economy;

namespace Gacha.Tests.Tools
{
    /// <summary>
    /// Phase 2 gate (plan doc gate 2): a player who collects a full idle chest once a day, clears every stage their level
    /// allows (stage about level x 3, the campaign's expected-level curve) and spends everything on the five Contract
    /// slots. Reports idle hours until all five reach level 200 against the pacing target (700 h, pass within 20%).
    /// Resource pacing only: star caps are assumed to keep up (copies arrive with summoning in phase 3).
    /// Run: pwsh -File tools/csharp/test.ps1 -Main Gacha.Tests.Tools.EconomySim   (args: targetHours=700, xpMult=1, clearMult=1 — the last two are what-ifs)
    /// </summary>
    public static class EconomySim
    {
        public static int Main(string[] args)
        {
            double target = args.Length > 0 ? double.Parse(args[0]) : 700;
            double xpMult = args.Length > 1 ? double.Parse(args[1], System.Globalization.CultureInfo.InvariantCulture) : 1;   // what-if: scale level XP costs
            double clearMult = args.Length > 2 ? double.Parse(args[2], System.Globalization.CultureInfo.InvariantCulture) : 1; // what-if: scale first-clear rewards
            var g = TestData.Game;
            var w = new Wallet();
            int level = 1, highest = 0, days = 0;
            Console.WriteLine("| Day | Idle hours | Level | Highest stage | Expected level there | Gold/h |");
            Console.WriteLine("| --- | --- | --- | --- | --- | --- |");
            while (level < 200 && days < 400)
            {
                // Clear every stage the team is levelled for, taking first-clear rewards.
                while (highest < g.Stages.Count && Expected.Level(highest + 1) <= level)
                {
                    highest++;
                    foreach (var kv in g.Stages[highest - 1].FirstClear) w.Add(kv.Key, kv.Value * clearMult);
                }
                // A day of idle loot, collected once.
                days++;
                foreach (var res in g.Idle.Resources) w.Add(res, g.Idle.PerHour(res, highest) * 24);
                // Level all five slots together while affordable.
                while (level < 200)
                {
                    int next = level + 1;
                    double xp = 5 * g.Levels.Xp(next) * xpMult, gold = 5 * g.Levels.Gold(next), star = 5 * g.Levels.Starlight(next);
                    if (w.Get("heroXp") < xp || w.Get("gold") < gold || w.Get("starlight") < star) break;
                    w.Add("heroXp", -xp); w.Add("gold", -gold); w.Add("starlight", -star);
                    level = next;
                }
                if (days % 5 == 0 || level >= 200)
                    Console.WriteLine($"| {days} | {days * 24} | {level} | {highest} | {Expected.Level(Math.Max(1, highest))} | {g.Idle.PerHour("gold", highest):N0} |");
            }
            double hours = days * 24.0, off = hours / target - 1;
            bool pass = level >= 200 && Math.Abs(off) <= 0.20;
            Console.WriteLine();
            Console.WriteLine($"Level 200 after {hours:N0} idle hours ({days} days); target {target:N0} h; {off:+0%;-0%} off. {(pass ? "GATE PASSED" : "GATE FAILED")}");
            Console.WriteLine($"Left over: {w.Get("gold"):N0} gold, {w.Get("heroXp"):N0} Hero XP, {w.Get("starlight"):N0} Starlight (stars and gear also spend gold).");
            return pass ? 0 : 1;
        }
    }
}
