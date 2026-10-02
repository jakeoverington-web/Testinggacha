using System;
using System.Collections.Generic;
using System.Linq;
using Gacha.Core.Battle;

namespace Gacha.Tests
{
    /// <summary>
    /// One-line battle builder for tests: Lab.Seed(1).Ally("halcyra").Enemy("dummy").Run(10).
    /// "dummy" is a passive training target (lots of HP, no attacks, no skills) defined in TestData.
    /// </summary>
    public sealed class Lab
    {
        public static GameData Data => TestData.Game;
        readonly List<string> _a = new List<string>(), _b = new List<string>();
        ulong _seed = 1;
        public Battle Battle;

        public static Lab Seed(ulong seed) => new Lab { _seed = seed };
        public Lab Ally(params string[] ids) { _a.AddRange(ids); return this; }
        public Lab Enemy(params string[] ids) { _b.AddRange(ids); return this; }

        public Battle Build()
        {
            Battle = new Battle(Data, new TeamSetup(_a.ToArray()), new TeamSetup(_b.ToArray()), _seed);
            return Battle;
        }

        public Lab Run(double seconds)
        {
            if (Battle == null) Build();
            Battle.RunFor(seconds);
            return this;
        }

        public Unit A(int i = 0) => Battle.Units.Where(u => u.Team == 0 && u.IsHero).ElementAt(i);
        public Unit B(int i = 0) => Battle.Units.Where(u => u.Team == 1 && u.IsHero).ElementAt(i);
        public IEnumerable<BattleEvent> Events(Ev type) => Battle.Log.Where(e => e.Type == type);
        public IEnumerable<BattleEvent> From(Unit u, Ev type) => Battle.Log.Where(e => e.Type == type && e.Src == u.Index);
        public IEnumerable<BattleEvent> To(Unit u, Ev type) => Battle.Log.Where(e => e.Type == type && e.Dst == u.Index);
        public bool Cast(Unit u, string key) => Battle.Log.Any(e => e.Type == Ev.Cast && e.Src == u.Index && e.What == key);
    }
}
