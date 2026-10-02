using System;
using System.Collections.Generic;
using System.IO;
using Gacha.Core.Data;

namespace Gacha.Core.Battle
{
    /// <summary>Combat stats (combat.json "stats"). Plain numbers; buffs are applied on top at runtime.</summary>
    public sealed class Stats
    {
        public double Hp, Atk, Def, AtkSpd, MoveSpd = 3, Range = 1, CritRate = 0.05, CritDmg = 1.5, Haste, EnergyRegen,
            Accuracy, Dodge, Block, EffectHit, EffectRes = 0.15, Lifesteal, HealPower, HealRecv;

        public static Stats From(Node n)
        {
            var s = new Stats();
            if (n == null) return s;
            s.Hp = n.Num("hp"); s.Atk = n.Num("atk"); s.Def = n.Num("def"); s.AtkSpd = n.Num("atkSpd", 1);
            s.MoveSpd = n.Num("moveSpd", 3); s.Range = n.Num("range", 1); s.CritRate = n.Num("critRate", 0.05);
            s.CritDmg = n.Num("critDmg", 1.5); s.Haste = n.Num("haste"); s.EnergyRegen = n.Num("energyRegen");
            s.Accuracy = n.Num("accuracy"); s.Dodge = n.Num("dodge"); s.Block = n.Num("block"); s.EffectHit = n.Num("effectHit");
            s.EffectRes = n.Num("effectRes", 0.15); s.Lifesteal = n.Num("lifesteal"); s.HealPower = n.Num("healPower"); s.HealRecv = n.Num("healRecv");
            return s;
        }

        public Stats Clone() => (Stats)MemberwiseClone();
    }

    public sealed class AbilityDef
    {
        public string Key;          // "ult", "s1", "s2"
        public string Name;
        public double Cooldown;     // 0 for the ultimate (energy based)
        public List<Node> Ops = new List<Node>();
        public string Requires;     // e.g. "dead_ally": waits until true
        public List<string> Tags = new List<string>();
    }

    public sealed class ModDef
    {
        public string K, If, Scope = "self", PerStack;
        public double V;
        public int PerBuff;
        public static ModDef From(Node n) => new ModDef
        {
            K = n.Str("k"), V = n.Num("v"), If = n.Str("if"), Scope = n.Str("scope", "self"),
            PerStack = n.Str("perStack"), PerBuff = (int)n.Num("perBuff")
        };
    }

    public sealed class TriggerDef
    {
        public string On, If;
        public double Icd;
        public List<Node> Ops = new List<Node>();
    }

    public sealed class PassiveDef
    {
        public string Name;
        public List<ModDef> Mods = new List<ModDef>();
        public List<TriggerDef> Triggers = new List<TriggerDef>();
        public HashSet<string> Flags = new HashSet<string>();
    }

    public sealed class HeroDef
    {
        public string Id, Name, Core, Subrace, Role, Range, Style;
        public Stats Stats;
        public AbilityDef Ult, S1, S2;
        public PassiveDef Passive = new PassiveDef();
        public string EnemyAi = "nearest", AllyAi = "self", Move = "advance";
        public bool IsMelee => Range == "melee";

        public static HeroDef From(Node h)
        {
            var d = new HeroDef
            {
                Id = h.Str("id"), Name = h.Str("name"), Core = h.Str("core"), Subrace = h.Str("subrace"),
                Role = h.Str("role"), Range = h.Str("range"), Style = h.Str("style"), Stats = Stats.From(h.Obj("stats"))
            };
            var sk = h.Obj("skills");
            d.Ult = Ability(sk.Obj("ultimate"), "ult");
            d.S1 = Ability(sk.Obj("skill1"), "s1");
            d.S2 = Ability(sk.Obj("skill2"), "s2");
            var p = sk.Obj("passive");
            d.Passive.Name = p.Str("name");
            foreach (var m in p.Nodes("mods")) d.Passive.Mods.Add(ModDef.From(m));
            foreach (var t in p.Nodes("triggers"))
                d.Passive.Triggers.Add(new TriggerDef { On = t.Str("on"), If = t.Str("if"), Icd = t.Num("icd"), Ops = t.Nodes("ops") });
            foreach (var f in p.Strs("flags")) d.Passive.Flags.Add(f);
            var ai = h.Obj("ai");
            if (ai != null) { d.EnemyAi = ai.Str("enemy", "nearest"); d.AllyAi = ai.Str("ally", "self"); d.Move = ai.Str("move", "advance"); }
            return d;
        }

        static AbilityDef Ability(Node n, string key) => new AbilityDef
        {
            Key = key, Name = n.Str("name"), Cooldown = n.Num("cooldown"), Ops = n.Nodes("ops"),
            Requires = n.Str("requires"), Tags = n.Strs("tags")
        };
    }

    public sealed class SummonDef
    {
        public string Id;
        public double HpPct, AtkPct, Def, AtkSpd, Range, MoveSpd;
        public bool Untargetable, Permanent;
        public List<ModDef> Mods = new List<ModDef>();
    }

    /// <summary>battle.json: every tunable number of the sim.</summary>
    public sealed class BattleTuning
    {
        public double Tick = 0.1, TimeLimit = 90, HalfWidth = 8, HalfDepth = 3, DefK = 300, InitialCooldownPct = 0.5, CastLock = 0.3,
            MeleeRangeMax = 2, HasteEnergyFactor = 0.5, ShieldDuration = 10, ShieldCap = 0.5, DrWindow = 6, DrHalfFor = 4,
            SleepCritBonus = 0.3, BleedMovingMult = 2, SoakFire = -0.15, SoakLightning = 0.25, GroupedR = 2, IsolatedR = 3, DensestR = 2,
            HealSkillThreshold = 0.95, ResistFloor = 0.15, ControlMax = 2.5, DiveDelay = 3, HpScale = 1, BasicPct = 1, BreathRadius = 1.5, BreathBurnV = 0.2, BreathBurnDur = 3;
        public int DotMaxStacks = 3;
        public List<int[]> Formation = new List<int[]>();
        public double[] ColX = { 2.5, 5.0 }, RowY = { -2.4, -1.2, 0, 1.2, 2.4 };
        public List<string> HardControl = new List<string>();
        public Dictionary<string, double> StatusDefaults = new Dictionary<string, double>();
        public Dictionary<string, SummonDef> Summons = new Dictionary<string, SummonDef>();

        public static BattleTuning From(Node n)
        {
            var t = new BattleTuning
            {
                Tick = n.Num("tick", 0.1), TimeLimit = n.Num("timeLimit", 90), DefK = n.Num("defK", 300),
                InitialCooldownPct = n.Num("initialCooldownPct", 0.5), CastLock = n.Num("castLock", 0.3), MeleeRangeMax = n.Num("meleeRangeMax", 2),
                HasteEnergyFactor = n.Num("hasteEnergyFactor", 0.5), ShieldDuration = n.Num("shieldDuration", 10), ShieldCap = n.Num("shieldCapPctMaxHp", 0.5),
                DotMaxStacks = (int)n.Num("dotMaxStacks", 3), BleedMovingMult = n.Num("bleedMovingMult", 2), HealSkillThreshold = n.Num("healSkillThreshold", 0.95), ResistFloor = n.Num("resistFloor", 0.15), DiveDelay = n.Num("diveDelay", 3), HpScale = n.Num("hpScale", 1), BasicPct = n.Num("basicAttackPct", 1)
            };
            var f = n.Obj("field"); t.HalfWidth = f.Num("halfWidth", 8); t.HalfDepth = f.Num("halfDepth", 3);
            var fm = n.Obj("formation");
            foreach (var cell in fm.List("default")) { var l = (List<object>)cell; t.Formation.Add(new[] { (int)(double)l[0], (int)(double)l[1] }); }
            t.ColX = Nums(fm.List("colX")); t.RowY = Nums(fm.List("rowY"));
            var c = n.Obj("control");
            t.HardControl = c.Strs("hard"); t.DrWindow = c.Num("drWindow", 6); t.DrHalfFor = c.Num("drHalfFor", 4); t.SleepCritBonus = c.Num("sleepCritBonus", 0.3); t.ControlMax = c.Num("maxDuration", 2.5);
            var sd = n.Obj("statusDefaults"); foreach (var k in sd.Keys) t.StatusDefaults[k] = sd.Num(k);
            var so = n.Obj("soaked"); t.SoakFire = so.Num("fire", -0.15); t.SoakLightning = so.Num("lightning", 0.25);
            t.GroupedR = n.Obj("grouped").Num("radius", 2); t.IsolatedR = n.Obj("isolated").Num("radius", 3); t.DensestR = n.Obj("densest").Num("radius", 2);
            var df = n.Obj("dragonform"); t.BreathRadius = df.Num("breathRadius", 1.5); t.BreathBurnV = df.Num("burnV", 0.2); t.BreathBurnDur = df.Num("burnDur", 3);
            var sm = n.Obj("summons");
            foreach (var k in sm.Keys)
            {
                var s = sm.Obj(k);
                var def = new SummonDef
                {
                    Id = k, HpPct = s.Num("hpPct"), AtkPct = s.Num("atkPct"), Def = s.Num("def"), AtkSpd = s.Num("atkSpd"), Range = s.Num("range"),
                    MoveSpd = s.Num("moveSpd"), Untargetable = s.Bool("untargetable"), Permanent = s.Bool("permanent")
                };
                foreach (var m in s.Nodes("mods")) def.Mods.Add(ModDef.From(m));
                t.Summons[k] = def;
            }
            return t;
        }

        static double[] Nums(List<object> l) { var r = new double[l.Count]; for (int i = 0; i < l.Count; i++) r[i] = (double)l[i]; return r; }
        public double DefaultDur(string status, double fallback) => StatusDefaults.TryGetValue(status, out var d) ? d : fallback;
    }

    /// <summary>Everything the battle reads, loaded once from Assets/_Game/Data.</summary>
    public sealed class GameData
    {
        public readonly Dictionary<string, HeroDef> Heroes = new Dictionary<string, HeroDef>();
        public readonly List<string> HeroOrder = new List<string>();
        public readonly Dictionary<string, string> Counters = new Dictionary<string, string>();   // core -> core it beats
        public BattleTuning Tuning;
        public double CounterBonus = 0.10;
        /// <summary>Team bonus: stat -> fraction. Null until tuned (races.json teamBonus is null = no bonus).</summary>
        public Dictionary<string, double> BonusThreePlusTwo, BonusFiveOfOne;

        public static GameData Load(string dataDir) => FromJson(
            File.ReadAllText(Path.Combine(dataDir, "heroes.json")),
            File.ReadAllText(Path.Combine(dataDir, "battle.json")),
            File.ReadAllText(Path.Combine(dataDir, "races.json")));

        public static GameData FromJson(string heroesJson, string battleJson, string racesJson)
        {
            var g = new GameData();
            foreach (var h in Node.Of(Json.Parse(heroesJson)).Nodes("heroes"))
            {
                var d = HeroDef.From(h);
                g.Heroes[d.Id] = d; g.HeroOrder.Add(d.Id);
            }
            g.Tuning = BattleTuning.From(Node.Of(Json.Parse(battleJson)));
            var races = Node.Of(Json.Parse(racesJson));
            foreach (var c in races.Nodes("cores")) g.Counters[c.Str("id")] = c.Str("counters");
            g.CounterBonus = races.Num("counterDamageBonus", 0.10);
            var tb = races.Obj("teamBonus");
            if (tb != null) { g.BonusThreePlusTwo = StatMap(tb.Obj("threePlusTwo")); g.BonusFiveOfOne = StatMap(tb.Obj("fiveOfOneCore")); }
            return g;
        }

        static Dictionary<string, double> StatMap(Node n)
        {
            if (n == null) return null;
            var d = new Dictionary<string, double>();
            foreach (var k in n.Keys) if (n.Raw[k] is double v) d[k] = v;
            return d;
        }

        /// <summary>Walks up from the working directory to find Assets/_Game/Data (works in Unity and in tools/csharp).</summary>
        public static string FindDataDir()
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            while (dir != null)
            {
                var p = Path.Combine(dir.FullName, "Assets", "_Game", "Data");
                if (Directory.Exists(p)) return p;
                dir = dir.Parent;
            }
            throw new DirectoryNotFoundException("Assets/_Game/Data not found above " + Environment.CurrentDirectory);
        }
    }
}
