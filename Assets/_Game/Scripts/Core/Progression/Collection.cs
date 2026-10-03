using System;
using System.Collections.Generic;
using Gacha.Core.Battle;
using Gacha.Core.Economy;

namespace Gacha.Core.Progression
{
    public sealed class OwnedHero
    {
        public string Id;
        public int Stars = 1;
        /// <summary>Spare duplicates not yet spent on stars.</summary>
        public int Copies;
    }

    /// <summary>A Contract slot: one of the top five. The level belongs to the slot, not the hero (phase 2 spec).</summary>
    public sealed class ContractSlot
    {
        public string Hero = "";
        public int Level = 1;
    }

    /// <summary>
    /// The player's heroes (decisions rows 7, 31; phase 2 spec). Contract slots hold the top five; a hero in a slot has the
    /// slot's level capped by her stars; everyone else takes the lowest slot's level whatever her stars. Swapping is free.
    /// </summary>
    public sealed class Collection
    {
        public const int SlotCount = 5;

        public readonly Dictionary<string, OwnedHero> Heroes = new Dictionary<string, OwnedHero>();
        /// <summary>Race Sigils by race id (high, dark, nature, ocean, arcane): one counts as one fodder of that race (row 32).</summary>
        public readonly Dictionary<string, int> Sigils = new Dictionary<string, int>();
        public readonly ContractSlot[] Slots = new ContractSlot[SlotCount];

        public Collection() { for (int i = 0; i < SlotCount; i++) Slots[i] = new ContractSlot(); }

        public bool Owns(string id) => Heroes.ContainsKey(id);
        public OwnedHero Get(string id) => Heroes.TryGetValue(id, out var h) ? h : null;

        /// <summary>A new hero arrives at 1 star; a hero already owned becomes a spare copy.</summary>
        public void Add(string id, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                if (Heroes.TryGetValue(id, out var h)) h.Copies++;
                else Heroes[id] = new OwnedHero { Id = id };
            }
        }

        public int SigilsOf(string race) => Sigils.TryGetValue(race, out var n) ? n : 0;
        public void AddSigils(string race, int n) => Sigils[race] = SigilsOf(race) + n;

        public bool InSlot(string id) => SlotOf(id) >= 0;
        public int SlotOf(string id) { for (int i = 0; i < SlotCount; i++) if (Slots[i].Hero == id) return i; return -1; }

        /// <summary>The level everyone outside the slots has: the lowest slot's level.</summary>
        public int SyncLevel { get { int m = int.MaxValue; foreach (var s in Slots) m = Math.Min(m, s.Level); return m; } }

        public int Level(GameData g, string id)
        {
            int slot = SlotOf(id);
            if (slot < 0) return SyncLevel;
            return Math.Min(Slots[slot].Level, g.Levels.Cap(Get(id)?.Stars ?? 1));
        }

        /// <summary>Puts an owned hero in a slot (free, no cooldown). She leaves any slot she was in; the slot keeps its level.</summary>
        public void Contract(int slot, string id)
        {
            if (!Owns(id)) throw new InvalidOperationException("Hero not owned: " + id);
            int was = SlotOf(id);
            if (was >= 0) Slots[was].Hero = "";
            Slots[slot].Hero = id;
        }

        public void Uncontract(int slot) => Slots[slot].Hero = "";

        /// <summary>Raises a slot one level, spending Hero XP, gold and (every 20th level) Starlight. False if empty, capped or short.</summary>
        public bool LevelUp(GameData g, int slot, Wallet w)
        {
            var s = Slots[slot];
            if (s.Hero == "") return false;
            int next = s.Level + 1;
            if (next > g.Levels.Cap(Get(s.Hero).Stars)) return false;
            double xp = g.Levels.Xp(next), gold = g.Levels.Gold(next), star = g.Levels.Starlight(next);
            if (w.Get("heroXp") < xp || w.Get("gold") < gold || w.Get("starlight") < star) return false;
            w.Add("heroXp", -xp); w.Add("gold", -gold); if (star > 0) w.Add("starlight", -star);
            s.Level = next;
            return true;
        }

        /// <summary>Sets a slot back to level 1 and refunds everything spent on it (free reset, row 31).</summary>
        public void ResetSlot(GameData g, int slot, Wallet w)
        {
            var s = Slots[slot];
            for (int l = 2; l <= s.Level; l++)
            {
                w.Add("heroXp", g.Levels.Xp(l)); w.Add("gold", g.Levels.Gold(l));
                int star = g.Levels.Starlight(l); if (star > 0) w.Add("starlight", star);
            }
            s.Level = 1;
        }
    }
}
