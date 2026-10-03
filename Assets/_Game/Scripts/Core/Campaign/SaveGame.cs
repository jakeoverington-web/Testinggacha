using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Gacha.Core.Data;

namespace Gacha.Core.Campaign
{
    /// <summary>
    /// The save file as JSON text (the Unity layer only reads and writes the file). A save that cannot be read
    /// starts a fresh game; a save from a newer version is refused rather than misread.
    /// </summary>
    public static class SaveGame
    {
        public const int Version = 1;

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
            }, pretty: true);
        }

        /// <param name="now">Used only when the save is missing or unreadable, to start a fresh chest and seed.</param>
        public static CampaignState Read(string json, long now)
        {
            if (string.IsNullOrWhiteSpace(json)) return Fresh(now);
            Node n;
            try { n = Node.Of(Json.Parse(json)); }
            catch (FormatException) { return Fresh(now); }
            if (n == null) return Fresh(now);
            int version = (int)n.Num("version", 0);
            if (version > Version) throw new InvalidDataException($"Save version {version} is newer than this build ({Version})");
            if (version < 1 || !(n.Raw.TryGetValue("highestCleared", out var hc) && hc is double)) return Fresh(now);

            var s = new CampaignState
            {
                Seed = ulong.TryParse(n.Str("seed"), NumberStyles.None, CultureInfo.InvariantCulture, out var seed) ? seed : (ulong)now,
                Attempts = (int)n.Num("attempts"),
                HighestCleared = (int)n.Num("highestCleared"),
                Auto = n.Bool("auto"),
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
            if (ls != null) foreach (var k in ls.Keys) s.LastSetup[k] = ls.Strs(k);
            return s;
        }

        static CampaignState Fresh(long now) => CampaignState.New((ulong)now, now);
    }
}
