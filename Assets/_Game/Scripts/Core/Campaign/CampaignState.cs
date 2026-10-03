using System;
using System.Collections.Generic;
using Gacha.Core.Battle;
using Gacha.Core.Economy;
using Gacha.Core.Gear;
using Gacha.Core.Progression;

namespace Gacha.Core.Campaign
{
    /// <summary>What the campaign expects at a stage (Game Modes and Progression Plan, Campaign: Difficulty).</summary>
    public static class Expected
    {
        public const int StagesPerChapter = 30;

        public static int Chapter(int stageIndex) => (stageIndex - 1) / StagesPerChapter + 1;

        /// <summary>About stage ÷ 3 (stage 300 is level 100), clamped to 1-200.</summary>
        public static int Level(int stageIndex) => Math.Clamp((int)Math.Round(stageIndex / 3.0), 1, 200);

        /// <summary>One more star every 4 chapters: 1★ in ch 1-4 … 5★ in ch 17-20.</summary>
        public static int Stars(int chapter) => 1 + (Math.Max(1, chapter) - 1) / 4;
    }

    /// <summary>
    /// Power (owner, 2026-10-03): sqrt(effective HP × damage output) on level/star-scaled stats, summed and rounded.
    /// EHP = HP × (1 + DEF / defK), matching the battle's k/(k+DEF) damage reduction; damage = ATK × attack speed ×
    /// (1 + crit rate × (crit damage − 1)). Skills are not counted, so damage dealers score highest.
    /// </summary>
    public static class TeamPower
    {
        public static double Hero(GameData g, string hero, int level, int stars, StatBonus bonus = null)
        {
            var s = g.Heroes[hero].Stats.Clone();
            double m = g.Progression.Mult(level, stars);
            s.Hp *= m; s.Atk *= m; s.Def *= m;
            bonus?.ApplyTo(s);
            double ehp = s.Hp * (1 + s.Def / g.Tuning.DefK);
            double dps = s.Atk * s.AtkSpd * (1 + System.Math.Min(1, s.CritRate) * (s.CritDmg - 1));
            return System.Math.Sqrt(System.Math.Max(0, ehp * dps));
        }

        public static long Of(GameData g, IList<string> heroes, int level, int stars)
        {
            double sum = 0;
            foreach (var h in heroes) sum += Hero(g, h, level, stars);
            return (long)Math.Round(sum);
        }

        public static long Of(GameData g, StageDef stage)
        {
            double sum = 0;
            foreach (var e in stage.Enemies) sum += Hero(g, e.Hero, e.Level, e.Stars);
            return (long)Math.Round(sum);
        }
    }

    public sealed class FightResult
    {
        public BattleResult Battle;
        public int StageIndex;
        public bool Won, FirstClear;
        /// <summary>Seed and setups, so the screen can replay the exact battle.</summary>
        public ulong Seed;
        public TeamSetup Player, Enemy;
        /// <summary>Gear dropped by this fight (a chapter boss's first clear).</summary>
        public List<GearItem> Gear = new List<GearItem>();
        /// <summary>Whether the fight started with manual ultimates, and the inputs applied (replay with Battle.Replay).</summary>
        public bool Manual;
        public IList<BattleInput> Inputs = new List<BattleInput>();
    }

    /// <summary>A fight in progress (redesign spec): the UI steps Battle in whole ticks and queues inputs, then calls FinishFight.</summary>
    public sealed class FightSession
    {
        public Battle.Battle Battle;
        public int StageIndex;
        public ulong Seed;
        public TeamSetup Player, Enemy;
        public bool Manual;
    }

    /// <summary>
    /// The player's campaign progress: highest stage, wallet, idle chest, auto mode, last-used team (row 18).
    /// Phase 1: the player's team fights at the expected level and stars of the stage (owner, 2026-10-03).
    /// </summary>
    public sealed class CampaignState
    {
        public const string Mode = "campaign";
        public const int MaxTeam = 5;

        public int HighestCleared;
        public Wallet Wallet = new Wallet();
        public IdleChest Chest = new IdleChest();
        public bool Auto;
        public Dictionary<string, List<string>> LastSetup = new Dictionary<string, List<string>>();
        public ulong Seed;
        public int Attempts;

        /// <summary>Phase 2: owned heroes, Contract slots and Sigils; gear; how many gear rolls have been made (seeds the next).</summary>
        public Collection Collection = new Collection();
        public Inventory Inventory = new Inventory();
        public int DropCounter;

        /// <summary>Tools and phase 1 tests: fight at the stage's expected level and stars with any hero, no gear (not saved).</summary>
        public bool UseExpectedLevels;

        /// <summary>A bare state in expected-level mode (tools, tests). The game uses NewGame.</summary>
        public static CampaignState New(ulong seed, long now) => new CampaignState { Seed = seed, Chest = IdleChest.StartAt(now), UseExpectedLevels = true };

        /// <summary>A new game: the seeded starter team owned at 1 star and placed in the Contract slots at level 1.</summary>
        public static CampaignState NewGame(GameData g, ulong seed, long now)
        {
            var st = new CampaignState { Seed = seed, Chest = IdleChest.StartAt(now) };
            st.GiveStarterTeam(g);
            return st;
        }

        public void GiveStarterTeam(GameData g)
        {
            var team = Starter.Roll(g, Seed);
            for (int i = 0; i < team.Count; i++) { Collection.Add(team[i]); Collection.Contract(i, team[i]); }
        }

        /// <summary>Ability ranks [ult, s1, s2, passive] at a star rank (plan doc table: 2-5 stars raise s1, s2, passive, ult to 2; 6-9 to 3).</summary>
        public static int[] RanksFor(int stars)
        {
            var r = new[] { 1, 1, 1, 1 };
            int[] order = { 1, 2, 3, 0 };   // s1, s2, passive, ult
            for (int s = 2; s <= Math.Min(stars, 9); s++) r[order[(s - 2) % 4]] = s <= 5 ? 2 : 3;
            return r;
        }

        public double HeroPower(GameData g, string hero)
        {
            var h = Collection.Get(hero);
            return TeamPower.Hero(g, hero, Collection.Level(g, hero), h?.Stars ?? 1, g.Gear != null ? Inventory.BonusFor(g.Gear, hero) : null);
        }

        /// <summary>Slots: up to 5 hero ids, "" = empty; slot i stands on battle.json default cell i.</summary>
        public TeamSetup PlayerSetup(GameData g, int stageIndex, IList<string> slots)
        {
            int level = Expected.Level(stageIndex), stars = Expected.Stars(Expected.Chapter(stageIndex));
            var t = new TeamSetup { Scales = new List<double>(), Cells = new List<int[]>() };
            if (!UseExpectedLevels) { t.Bonuses = new List<StatBonus>(); t.Ranks = new List<int[]>(); }
            for (int i = 0; i < slots.Count; i++)
            {
                string id = slots[i];
                if (string.IsNullOrEmpty(id)) continue;
                t.Heroes.Add(id);
                t.Cells.Add(g.Tuning.Formation[i]);
                if (UseExpectedLevels) { t.Scales.Add(g.Progression.Mult(level, stars)); continue; }
                int s = Collection.Get(id)?.Stars ?? 1;
                t.Scales.Add(g.Progression.Mult(Collection.Level(g, id), s));
                t.Bonuses.Add(g.Gear != null ? Inventory.BonusFor(g.Gear, id) : null);
                t.Ranks.Add(RanksFor(s));
            }
            return t;
        }

        /// <summary>Pads or reads a slot list to exactly MaxTeam entries ("" for empty).</summary>
        public static List<string> Slots(IList<string> slots)
        {
            var r = new List<string>();
            for (int i = 0; i < MaxTeam; i++) r.Add(slots != null && i < slots.Count && slots[i] != null ? slots[i] : "");
            return r;
        }

        public TeamSetup EnemySetup(GameData g, int stageIndex)
        {
            var stage = g.Stages[stageIndex - 1];
            var t = new TeamSetup { Scales = new List<double>() };
            foreach (var e in stage.Enemies) { t.Heroes.Add(e.Hero); t.Scales.Add(g.Progression.Mult(e.Level, e.Stars)); }
            return t;
        }

        /// <summary>Plays a stage to the end on auto (tools, auto-chaining). Same as StartFight + FinishFight.</summary>
        public FightResult Fight(GameData g, int stageIndex, IList<string> slots, long now) =>
            FinishFight(g, StartFight(g, stageIndex, slots, manual: false), now);

        /// <summary>Starts a live fight. Only cleared stages and the next one can be fought.</summary>
        public FightSession StartFight(GameData g, int stageIndex, IList<string> slots, bool manual)
        {
            int max = Math.Min(HighestCleared + 1, g.Stages.Count);
            if (stageIndex < 1 || stageIndex > max) throw new InvalidOperationException($"Stage {stageIndex} is locked (next is {HighestCleared + 1})");
            if (slots == null || slots.Count > MaxTeam) throw new ArgumentException("A team has at most 5 slots");
            var heroes = new List<string>();
            foreach (var h in slots) if (!string.IsNullOrEmpty(h)) heroes.Add(h);
            if (heroes.Count < 1) throw new ArgumentException("A team needs at least 1 hero");
            if (new HashSet<string>(heroes).Count != heroes.Count) throw new ArgumentException("A hero can only appear once in a team");
            foreach (var h in heroes) if (!g.Heroes.ContainsKey(h)) throw new ArgumentException("Unknown hero " + h);
            if (!UseExpectedLevels) foreach (var h in heroes) if (!Collection.Owns(h)) throw new ArgumentException("Hero not owned: " + h);

            ulong seed = new Rng(Seed ^ (ulong)Attempts).NextULong();
            Attempts++;
            LastSetup[Mode] = Slots(slots);
            var player = PlayerSetup(g, stageIndex, slots);
            var enemy = EnemySetup(g, stageIndex);
            var battle = new Battle.Battle(g, player, enemy, seed);
            battle.ManualUltimates[0] = manual;
            return new FightSession { Battle = battle, StageIndex = stageIndex, Seed = seed, Player = player, Enemy = enemy, Manual = manual };
        }

        /// <summary>Finishes a fight: an unfinished one runs to the end on auto (Skip, logged as SetAuto), then a first win pays once.</summary>
        public FightResult FinishFight(GameData g, FightSession s, long now)
        {
            var b = s.Battle;
            if (!b.Over)
            {
                if (b.ManualUltimates[0]) b.Queue(new BattleInput { Kind = InputKind.SetAuto, Team = 0, On = true });
                while (!b.Over) b.Step();
            }
            var battle = b.Result();
            var r = new FightResult
            {
                Battle = battle, StageIndex = s.StageIndex, Won = battle.Winner == 0, Seed = s.Seed, Player = s.Player, Enemy = s.Enemy,
                Manual = s.Manual, Inputs = new List<BattleInput>(b.Inputs)
            };
            if (r.Won && s.StageIndex == HighestCleared + 1)
            {
                Chest.Settle(now, HighestCleared, g.Idle);   // loot so far keeps the old rate
                HighestCleared = s.StageIndex;
                foreach (var kv in g.Stages[s.StageIndex - 1].FirstClear) Wallet.Add(kv.Key, kv.Value);
                r.FirstClear = true;
                if (g.Stages[s.StageIndex - 1].Boss && g.Gear != null && !UseExpectedLevels)
                    r.Gear.Add(GearDrops.Roll(DropRng(), g.Gear, GearDrops.UnlockedRarities(g.Gear, HighestCleared), Inventory));
            }
            return r;
        }

        /// <summary>Auto mode (row 21): keep going after a win, stop on a loss or after the last stage.</summary>
        public bool AutoContinues(GameData g, FightResult r) => Auto && r.Won && r.StageIndex < g.Stages.Count;

        /// <summary>Collects the idle chest; outside expected-level mode it also rolls idle gear for the chest's hours.</summary>
        public List<GearItem> Collect(GameData g, long now)
        {
            Chest.Settle(now, HighestCleared, g.Idle);
            double hours = Chest.AccruedSeconds / 3600;
            Chest.Collect(now, HighestCleared, g.Idle, Wallet);
            var got = new List<GearItem>();
            if (UseExpectedLevels || g.Gear == null || HighestCleared <= 0) return got;
            var rng = DropRng();
            int n = GearDrops.IdleCount(rng, g.Gear, hours);
            int unlocked = GearDrops.UnlockedRarities(g.Gear, HighestCleared);
            for (int i = 0; i < n; i++) got.Add(GearDrops.Roll(rng, g.Gear, unlocked, Inventory));
            return got;
        }

        /// <summary>A fresh seeded stream for each drop event (save seed + counter), so drops replay exactly.</summary>
        Rng DropRng() => new Rng(new Rng(Seed ^ 0xD80F5EEDUL ^ ((ulong)DropCounter++ * 0x9E3779B97F4A7C15UL)).NextULong());

    }
}
