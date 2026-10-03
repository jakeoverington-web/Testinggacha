using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Gacha.Core.Battle;
using Gacha.Core.Data;
using Gacha.Core.Gear;
using Gacha.Core.Progression;

namespace Gacha.Core.Campaign
{
    /// <summary>
    /// The save file as JSON text (the Unity layer only reads and writes the file). A save that cannot be read
    /// starts a fresh game; a save from a newer version is refused rather than misread.
    /// </summary>
    public static class SaveGame
    {
        public const int Version = 2;   // 2: collection, Contract slots, Sigils, gear (phase 2)

        public static string Write(CampaignState s)
        {
            var wallet = new Dictionary<string, object>();
            foreach (var kv in s.Wallet.Amounts) wallet[kv.Key] = kv.Value;
            var held = new Dictionary<string, object>();
            foreach (var kv in s.Chest.Held) held[kv.Key] = kv.Value;
            var setups = new Dictionary<string, object>();
            foreach (var kv in s.LastSetup) setups[kv.Key] = new List<object>(kv.Value);
            return Json.Write(new Dictionary<string, object>
            {
                ["version"] = Version,
                ["seed"] = s.Seed.ToString(CultureInfo.InvariantCulture),   // a ulong does not fit a JSON number exactly
                ["attempts"] = s.Attempts,
                ["highestCleared"] = s.HighestCleared,
                ["auto"] = s.Auto,
                ["wallet"] = wallet,
                ["chest"] = new Dictionary<string, object> { ["lastSettled"] = s.Chest.LastSettled, ["accruedSeconds"] = s.Chest.AccruedSeconds, ["held"] = held },
                ["lastSetup"] = setups,
                ["collection"] = WriteCollection(s.Collection),
                ["gear"] = WriteGear(s.Inventory),
                ["dropCounter"] = s.DropCounter,
            }, pretty: true);
        }

        static Dictionary<string, object> WriteCollection(Collection c)
        {
            var heroes = new Dictionary<string, object>();
            foreach (var kv in c.Heroes) heroes[kv.Key] = new Dictionary<string, object> { ["stars"] = kv.Value.Stars, ["copies"] = kv.Value.Copies };
            var sigils = new Dictionary<string, object>();
            foreach (var kv in c.Sigils) sigils[kv.Key] = kv.Value;
            var slots = new List<object>();
            foreach (var sl in c.Slots) slots.Add(new Dictionary<string, object> { ["hero"] = sl.Hero, ["level"] = sl.Level });
            return new Dictionary<string, object> { ["heroes"] = heroes, ["sigils"] = sigils, ["slots"] = slots };
        }

        static Dictionary<string, object> WriteGear(Inventory inv)
        {
            var items = new List<object>();
            foreach (var i in inv.Items)
                items.Add(new Dictionary<string, object> { ["id"] = i.Id, ["type"] = i.Type, ["set"] = i.Set, ["rarity"] = i.Rarity, ["upgrade"] = i.Upgrade, ["on"] = i.EquippedOn });
            return new Dictionary<string, object> { ["nextId"] = inv.NextId, ["items"] = items };
        }

        /// <param name="now">Used only when the save is missing or unreadable, to start a fresh chest and seed.</param>
        /// <param name="g">Game data: with it, a fresh or version-1 save gets the seeded starter team (the game);
        /// without it, the state stays in expected-level mode (tools and phase 1 tests).</param>
        public static CampaignState Read(string json, long now, GameData g = null)
        {
            if (string.IsNullOrWhiteSpace(json)) return Fresh(now, g);
            try { return ReadNode(Node.Of(Json.Parse(json)), now, g); }
            catch (InvalidDataException) { throw; }
            catch (Exception) { return Fresh(now, g); }   // truncated or damaged file (the parser can throw more than FormatException)
        }

        static CampaignState ReadNode(Node n, long now, GameData g)
        {
            if (n == null) return Fresh(now, g);
            int version = (int)n.Num("version", 0);
            if (version > Version) throw new InvalidDataException($"Save version {version} is newer than this build ({Version})");
            if (version < 1 || !(n.Raw.TryGetValue("highestCleared", out var hc) && hc is double)) return Fresh(now, g);

            var s = new CampaignState
            {
                Seed = ulong.TryParse(n.Str("seed"), NumberStyles.None, CultureInfo.InvariantCulture, out var seed) ? seed : (ulong)now,
                Attempts = (int)n.Num("attempts"),
                HighestCleared = (int)n.Num("highestCleared"),
                Auto = n.Bool("auto"),
                DropCounter = (int)n.Num("dropCounter"),
                UseExpectedLevels = g == null,
            };
            var w = n.Obj("wallet");
            if (w != null) foreach (var k in w.Keys) s.Wallet.Add(k, w.Num(k));
            var c = n.Obj("chest");
            s.Chest.LastSettled = c != null ? (long)c.Num("lastSettled", now) : now;
            if (c != null)
            {
                s.Chest.AccruedSeconds = c.Num("accruedSeconds");
                var held = c.Obj("held");
                if (held != null) foreach (var k in held.Keys) s.Chest.Held[k] = held.Num(k);
            }
            var ls = n.Obj("lastSetup");
            if (ls != null) foreach (var k in ls.Keys) s.LastSetup[k] = CampaignState.Slots(ls.Strs(k));   // old saves: a plain hero list fills slots in order
            var col = n.Obj("collection");
            if (col != null) ReadCollection(col, s.Collection);
            else if (g != null) s.GiveStarterTeam(g);   // version 1 save: keep progress, roll the starter team
            var gear = n.Obj("gear");
            if (gear != null)
            {
                foreach (var i in gear.Nodes("items"))
                    s.Inventory.Items.Add(new GearItem { Id = (int)i.Num("id"), Type = i.Str("type"), Set = i.Str("set"), Rarity = (int)i.Num("rarity", 1), Upgrade = (int)i.Num("upgrade"), EquippedOn = i.Str("on", "") });
                s.Inventory.NextId = (int)gear.Num("nextId", s.Inventory.Items.Count + 1);
            }
            return s;
        }

        static void ReadCollection(Node n, Collection c)
        {
            var heroes = n.Obj("heroes");
            if (heroes != null)
                foreach (var k in heroes.Keys)
                {
                    var h = heroes.Obj(k);
                    c.Heroes[k] = new OwnedHero { Id = k, Stars = (int)h.Num("stars", 1), Copies = (int)h.Num("copies") };
                }
            var sig = n.Obj("sigils");
            if (sig != null) foreach (var k in sig.Keys) c.Sigils[k] = (int)sig.Num(k);
            var slots = n.Nodes("slots");
            for (int i = 0; i < slots.Count && i < Collection.SlotCount; i++)
            {
                string hero = slots[i].Str("hero", "");
                c.Slots[i].Hero = c.Owns(hero) ? hero : "";
                c.Slots[i].Level = Math.Max(1, (int)slots[i].Num("level", 1));
            }
        }

        static CampaignState Fresh(long now, GameData g) => g != null ? CampaignState.NewGame(g, (ulong)now, now) : CampaignState.New((ulong)now, now);
    }
}
