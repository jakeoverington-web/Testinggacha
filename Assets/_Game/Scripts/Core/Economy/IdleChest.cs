using System;
using System.Collections.Generic;

namespace Gacha.Core.Economy
{
    /// <summary>
    /// The idle chest (decisions row 33). Time comes in as unix seconds; Core never reads a clock.
    /// Settle at the old stage before the highest stage changes, so earlier loot keeps the old rate.
    /// The clock never runs backwards for the chest: an earlier time adds nothing and is ignored.
    /// </summary>
    public sealed class IdleChest
    {
        public long LastSettled;
        /// <summary>Seconds of loot in the chest since the last collect (stops at the cap).</summary>
        public double AccruedSeconds;
        public readonly Dictionary<string, double> Held = new Dictionary<string, double>();

        public static IdleChest StartAt(long now) => new IdleChest { LastSettled = now };

        public double Get(string res) => Held.TryGetValue(res, out var v) ? v : 0;

        public void Settle(long now, int highestStage, IdleRates rates)
        {
            long dt = now - LastSettled;
            if (dt <= 0) return;
            LastSettled = now;
            if (highestStage <= 0) return;   // nothing earns yet, so the cap is not used up
            double take = Math.Min(dt, rates.CapHours * 3600 - AccruedSeconds);
            if (take <= 0) return;
            AccruedSeconds += take;
            foreach (var res in rates.Resources) Held[res] = Get(res) + rates.PerHour(res, highestStage) * take / 3600;
        }

        public void Collect(long now, int highestStage, IdleRates rates, Wallet wallet)
        {
            Settle(now, highestStage, rates);
            foreach (var kv in Held) wallet.Add(kv.Key, kv.Value);
            Held.Clear();
            AccruedSeconds = 0;
        }

        public void UseHourglass(int highestStage, IdleRates rates, Wallet wallet)
        {
            foreach (var res in rates.Resources) wallet.Add(res, rates.PerHour(res, highestStage) * rates.HourglassHours);
        }
    }
}
