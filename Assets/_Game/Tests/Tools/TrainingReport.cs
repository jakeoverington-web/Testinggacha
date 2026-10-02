using System;
using System.Linq;
using Gacha.Core.Battle;

namespace Gacha.Tests.Tools
{
    /// <summary>
    /// Damage line: each hero with her own AI and passives, 30 s against five target dummies (DEF 120, no race),
    /// with four ally dummies being hit so healers and shielders have work. Shows what each kit really outputs.
    /// Run: tools/csharp/test.sh -Main Gacha.Tests.Tools.TrainingReport
    /// </summary>
    public static class TrainingReport
    {
        public static int Main(string[] args)
        {
            var D = TestData.Game;
            Console.WriteLine("| Hero | Role | Damage / 30 s | Heal + shield / 30 s | Ults | Skill casts | Auto hits | Time in reach |");
            Console.WriteLine("| --- | --- | --- | --- | --- | --- | --- | --- |");
            foreach (var id in InvariantTests.Roster.OrderBy(h => D.Heroes[h].Role).ThenBy(h => h))
            {
                var b = new Battle(D, new TeamSetup(id, "ally_dummy", "ally_dummy", "ally_dummy", "ally_dummy"),
                    new TeamSetup("target_dummy", "target_dummy", "target_dummy", "target_dummy", "striker"), 9);
                var hero = b.Units[0];
                foreach (var e in b.Units.Where(u => u.Team == 1)) e.Base.Def = 120;
                var striker = b.Units.Last(u => u.Team == 1); striker.Base.Atk = 150; striker.Untargetable = true;
                double reach = 0;
                b.AfterTick = x =>
                {
                    var t = x.EnemyTarget(hero);
                    if (t != null && Battle.Dist(hero, t) <= hero.Base.Range + 0.25) reach += x.T.Tick;
                    if (!hero.Alive) { hero.Alive = true; hero.Hp = hero.MaxHp; }   // keep her in the lab
                };
                b.RunFor(30);
                var mine = b.Log.Where(e => e.Src >= 0 && (e.Src == hero.Index || (b.Units[e.Src].IsSummon && b.Units[e.Src].Owner == hero.Index))).ToList();
                double dmg = mine.Where(e => e.Type == Ev.Damage && b.Units[e.Dst].Team == 1).Sum(e => e.Amount);
                double sup = mine.Where(e => (e.Type == Ev.Heal || e.Type == Ev.ShieldGain) && b.Units[e.Dst].Team == 0).Sum(e => e.Amount);
                int ults = mine.Count(e => e.Type == Ev.Cast && e.What == "ult");
                int casts = mine.Count(e => e.Type == Ev.Cast && e.What != "ult");
                int autos = mine.Count(e => e.Type == Ev.Damage && e.Ab == "basic");
                Console.WriteLine($"| {D.Heroes[id].Name} | {D.Heroes[id].Role} | {dmg:0} | {sup:0} | {ults} | {casts} | {autos} | {100 * reach / 30:0}% |");
            }
            return 0;
        }
    }
}
