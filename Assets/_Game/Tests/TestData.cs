using System.IO;
using Gacha.Core.Battle;
using Gacha.Core.Data;

namespace Gacha.Tests
{
    /// <summary>Real game data plus test-only heroes ("dummy", "striker") appended to the hero list.</summary>
    public static class TestData
    {
        static GameData _game;
        public static GameData Game => _game ??= Load();
        public static string Dir => GameData.FindDataDir();

        static GameData Load()
        {
            var g = GameData.Load(Dir);
            g.Heroes["dummy"] = Dummy("dummy", "high", hp: 1_000_000, atk: 0, def: 0, atkSpd: 0);
            g.Heroes["dummy_dark"] = Dummy("dummy_dark", "dark", hp: 1_000_000, atk: 0, def: 0, atkSpd: 0);
            g.Heroes["dummy_def300"] = Dummy("dummy_def300", "high", hp: 1_000_000, atk: 0, def: 300, atkSpd: 0);
            g.Heroes["striker"] = Dummy("striker", "high", hp: 1_000_000, atk: 100, def: 0, atkSpd: 1);
            return g;
        }

        /// <summary>A hero with no skills: crit 0, resist 0, range 1, stands still unless it can attack.</summary>
        public static HeroDef Dummy(string id, string core, double hp, double atk, double def, double atkSpd)
        {
            var d = new HeroDef
            {
                Id = id, Name = id, Core = core, Role = "tank", Range = "melee",
                Stats = new Stats { Hp = hp, Atk = atk, Def = def, AtkSpd = atkSpd, MoveSpd = atkSpd > 0 ? 3 : 0, Range = 1, CritRate = 0, CritDmg = 1.5, EffectRes = 0 },
                Ult = new AbilityDef { Key = "ult" }, S1 = new AbilityDef { Key = "s1", Cooldown = 999 }, S2 = new AbilityDef { Key = "s2", Cooldown = 999 }
            };
            return d;
        }
    }
}
