using System;
using System.Collections.Generic;
using System.Linq;
using Gacha.Core;
using Gacha.Core.Battle;

namespace Gacha.Tests.Tools
{
    /// <summary>
    /// How heroes actually behave in real battles: lifetime, time spent able to hit, damage, casts, deaths.
    /// Run: tools/csharp/test.sh -Main Gacha.Tests.Tools.HeroProbe nyx caelith isaura   (defaults to all)
    /// </summary>
    public static class HeroProbe
    {
        public static int Main(string[] args)
        {
            var D = TestData.Game; var roster = InvariantTests.Roster;
            var ids = args.Length > 0 ? args.ToList() : roster;
            Console.WriteLine("| Hero | Battles | Lives (s, avg) | Dies first % | Damage / s alive | Ults / battle | Most ults in a battle | Skills / battle | Idle while alive % | Energy from: hits / damage taken / gifts |");
            Console.WriteLine("| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |");
            foreach (var id in ids)
            {
                var rng = new Rng(4242);
                double life = 0, dmg = 0, ults = 0, skills = 0, idle = 0, alive = 0, first = 0, eGift = 0; int n = 300; int maxU = 0; double eHit = 0, eTaken = 0;
                for (int i = 0; i < n; i++)
                {
                    var a = InvariantTests.RandomTeam(rng, roster.Where(h => h != id).ToList());
                    a.Heroes[0] = id;
                    var b = new Battle(D, a, InvariantTests.RandomTeam(rng, roster), rng.NextULong());
                    var me = b.Units[0];
                    double lastAct = 0; double myIdle = 0, myAlive = 0;
                    b.OnEvent = e => { if (e.Src == me.Index && (e.Type == Ev.Damage || e.Type == Ev.Cast || e.Type == Ev.Heal || e.Type == Ev.StatusOn)) lastAct = b.Time; };
                    b.AfterTick = x => { if (me.Alive) { myAlive += x.T.Tick; if (x.Time - lastAct > 2.0) myIdle += x.T.Tick; } };
                    b.AfterTick = x2 => { if (me.Alive) { myAlive += x2.T.Tick; if (x2.Time - lastAct > 2.0) myIdle += x2.T.Tick; } };
                    var r = b.Run();
                    var deaths = r.Log.Where(e => e.Type == Ev.Death && b.Units[e.Dst].IsHero && b.Units[e.Dst].Team == 0).ToList();
                    var myDeath = deaths.FirstOrDefault(e => e.Dst == me.Index);
                    if (deaths.Count > 0 && deaths[0].Dst == me.Index) first++;
                    life += myDeath.Type == Ev.Death ? myDeath.T : r.Time;
                    dmg += r.Log.Where(e => e.Type == Ev.Damage && (e.Src == me.Index || (e.Src >= 0 && b.Units[e.Src].IsSummon && b.Units[e.Src].Owner == me.Index)) && b.Units[e.Dst].Team == 1).Sum(e => e.Amount);
                    int u1 = r.Log.Count(e => e.Type == Ev.Cast && e.Src == me.Index && e.What == "ult"); ults += u1; maxU = Math.Max(maxU, u1);
                    eGift += r.Log.Where(e => e.Type == Ev.Energy && e.Dst == me.Index && e.Src != me.Index && e.Amount > 0).Sum(e => e.Amount);
                    eHit += 10 * r.Log.Count(e => e.Type == Ev.Damage && e.Src == me.Index && e.Ab == "basic");
                    eTaken += 0.5 * 100 * r.Log.Where(e => e.Type == Ev.Damage && e.Dst == me.Index).Sum(e => e.Amount) / me.MaxHp;
                    skills += r.Log.Count(e => e.Type == Ev.Cast && e.Src == me.Index && e.What != "ult");
                    idle += myIdle; alive += myAlive;
                }
                Console.WriteLine($"| {D.Heroes[id].Name} | {n} | {life / n:0.0} | {100 * first / n:0} | {dmg / Math.Max(1, alive):0} | {ults / n:0.0} | {maxU} | {skills / n:0.0} | {100 * idle / Math.Max(1, alive):0} | {eHit / n:0} / {eTaken / n:0} / {eGift / n:0} |");
            }
            return 0;
        }
    }
}
