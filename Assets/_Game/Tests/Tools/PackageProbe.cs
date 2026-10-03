using System;
using System.Collections.Generic;
using System.Linq;
using Gacha.Core;
using Gacha.Core.Battle;

namespace Gacha.Tests.Tools
{
    /// <summary>
    /// Why a synergy package wins or loses: in package battles (same teams as SynergyReport), how much of the team's damage
    /// lands while the keyword's condition is on the target (uptime), how much of that comes from the payers, and how the
    /// package heroes fare (damage share, first deaths).
    /// Run: tools/csharp/test.sh -Main Gacha.Tests.Tools.PackageProbe [battles=200] [keyword ...]
    /// </summary>
    public static class PackageProbe
    {
        /// <summary>The target condition each keyword's payoffs read (null = not a target condition).</summary>
        public static readonly Dictionary<string, string> Condition = new Dictionary<string, string> {
            ["Airborne"] = "tgt:airborne+3", ["Asleep"] = "tgt:sleep+3", ["Bleed"] = "tgt:bleed", ["Blind"] = "tgt:blind", ["Burn"] = "tgt:burn",
            ["Charmed"] = "tgt:charm+3", ["Curse"] = "tgt:curse", ["Drained"] = "tgt.energy<50", ["Feared"] = "tgt:fear+3", ["Grouped"] = "tgt.grouped",
            ["Isolated"] = "tgt.isolated", ["Mark"] = "tgt:mark", ["Rooted"] = "tgt:root+3", ["Slow"] = "tgt:slow", ["Soaked"] = "tgt:soaked",
            ["Stunned"] = "tgt:stun+3", ["Taunted"] = "tgt:taunt", ["Weakened"] = "tgt:weaken|def_down|dmg_down", ["Poison"] = "tgt:poison" };

        public static int Main(string[] args)
        {
            int n = args.Length > 0 && int.TryParse(args[0], out var nn) ? nn : 200;
            var kws = SynergyReport.Init();
            var only = args.Skip(1).ToList();
            if (only.Count == 1 && only[0] == "null")
            {
                // control experiment: fake keywords made of random heroes (3 appliers + 3 payers) measure the yardstick's own bias
                var nr = new Rng(31337); kws = new List<string>();
                for (int f = 0; f < 16; f++)
                {
                    var pick = SynergyReport.R.OrderBy(_ => nr.NextULong()).Take(6).ToList();
                    string k = "_null" + f; kws.Add(k);
                    SynergyReport.Applies[k] = pick.Take(3).ToList(); SynergyReport.Payoffs[k] = pick.Skip(3).ToList();
                }
            }
            else if (only.Count > 0) kws = kws.Where(only.Contains).ToList();
            var D = SynergyReport.D;
            var rows = new string[kws.Count];
            SynergyReport.Par(kws.Count, i =>
            {
                string kw = kws[i]; Condition.TryGetValue(kw, out var cond);
                var payers = SynergyReport.Payoffs[kw]; var appliers = SynergyReport.Applies[kw];
                var rng = new Rng(500UL + (ulong)i);
                double winsOff = 0, winsCtl = 0, all = 0, on = 0, pay = 0, payOn = 0, pkgDmg = 0, wins = 0, firstDeathPkg = 0, deaths = 0; int m = 0;
                var heroDmg = new Dictionary<string, double>(); var heroCount = new Dictionary<string, int>();
                for (int k = 0; k < n; k++)
                {
                    var team = SynergyReport.PackageTeam(rng, kw); if (team == null) break;
                    var ctl = SynergyReport.RandomTeam(rng); var opp = SynergyReport.RandomTeam(rng); ulong seed = rng.NextULong();
                    var b = new Battle(D, new TeamSetup(team.ToArray()), new TeamSetup(opp.ToArray()), seed) { KeepLog = true };
                    b.OnHit = tr =>
                    {
                        var s = b.Units[tr.Src]; if (s.Team != 0) return;
                        var owner = s.IsSummon ? b.Units[s.Owner] : s;
                        bool c = cond != null && b.Eval(cond, new Cond { Self = owner, Src = s, Tgt = b.Units[tr.Dst] });
                        all += tr.Final; if (c) on += tr.Final;
                        if (payers.Contains(owner.Id)) { pay += tr.Final; if (c) payOn += tr.Final; }
                        if (payers.Contains(owner.Id) || appliers.Contains(owner.Id)) pkgDmg += tr.Final;
                        heroDmg[owner.Id] = (heroDmg.TryGetValue(owner.Id, out var d) ? d : 0) + tr.Final;
                    };
                    var r = b.Run(); m++;
                    if (r.Winner == 0) wins++;
                    // the same battle with the payers' passives off, and the random control team
                    var bo = new Battle(D, new TeamSetup(team.ToArray()), new TeamSetup(opp.ToArray()), seed) { KeepLog = false };
                    for (int u = 0; u < team.Count; u++) if (payers.Contains(team[u])) bo.Units[u].Off = new HashSet<string> { "passive" };
                    var ro = bo.Run(); if (ro.Winner == 0) winsOff++; else if (ro.Winner < 0) winsOff += 0.5;
                    var rc = new Battle(D, new TeamSetup(ctl.ToArray()), new TeamSetup(opp.ToArray()), seed) { KeepLog = false }.Run();
                    if (rc.Winner == 0) winsCtl++; else if (rc.Winner < 0) winsCtl += 0.5;
                    if (r.Winner < 0) wins += 0.5;
                    foreach (var h in team) heroCount[h] = (heroCount.TryGetValue(h, out var c) ? c : 0) + 1;
                    var fd = r.Log.FirstOrDefault(e => e.Type == Ev.Death && b.Units[e.Dst].IsHero && b.Units[e.Dst].Team == 0);
                    if (fd.Type == Ev.Death) { deaths++; var id = b.Units[fd.Dst].Id; if (payers.Contains(id) || appliers.Contains(id)) firstDeathPkg++; }
                }
                // damage per appearance for package heroes
                var per = heroCount.Where(h => payers.Contains(h.Key) || appliers.Contains(h.Key))
                    .Select(h => (h.Key, dmg: (heroDmg.TryGetValue(h.Key, out var d) ? d : 0) / h.Value / Math.Max(1, all / m) * 100))
                    .OrderByDescending(x => x.dmg).Select(x => $"{D.Heroes[x.Key].Name} {x.dmg:0}");
                rows[i] = $"| {kw} | {100 * (wins - winsCtl) / Math.Max(1, m):+0;-0;0} | {100 * (wins - winsOff) / Math.Max(1, m):+0;-0;0} | {100 * (winsOff - winsCtl) / Math.Max(1, m):+0;-0;0} | {(cond == null ? "n/a" : (100 * on / Math.Max(1, all)).ToString("0"))} | {(cond == null ? "n/a" : (100 * payOn / Math.Max(1, pay)).ToString("0"))} | {100 * pay / Math.Max(1, all):0} | {100 * firstDeathPkg / Math.Max(1, deaths):0} | {string.Join(", ", per)} |";
            });
            Console.WriteLine($"_{n} package battles per keyword (same package teams as SynergyReport). Uptime = share of damage landing while the keyword is on the target._");
            Console.WriteLine();
            Console.WriteLine("| Keyword | Uplift vs random team | From payoffs (payers' passives on vs off) | Base (passives off vs random team) | Team damage with keyword on target % | Payers' damage with keyword on % | Payers' share of team damage % | First death is a package hero % | Package heroes: % of team damage per appearance |");
            Console.WriteLine("| --- | --- | --- | --- | --- | --- | --- | --- | --- |");
            foreach (var r in rows) Console.WriteLine(r);
            return 0;
        }
    }
}
