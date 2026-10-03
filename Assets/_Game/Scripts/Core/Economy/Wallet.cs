using System;
using System.Collections.Generic;

namespace Gacha.Core.Economy
{
    /// <summary>The player's resources by id (gold, heroXp, starlight, ...).</summary>
    public sealed class Wallet
    {
        public readonly Dictionary<string, double> Amounts = new Dictionary<string, double>();

        public double Get(string res) => Amounts.TryGetValue(res, out var v) ? v : 0;

        public void Add(string res, double amount)
        {
            if (double.IsNaN(amount) || double.IsInfinity(amount)) throw new ArgumentException($"Bad amount {amount} for {res}");
            Amounts[res] = Get(res) + amount;
        }
    }
}
