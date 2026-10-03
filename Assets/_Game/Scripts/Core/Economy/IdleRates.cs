using System;
using System.Collections.Generic;
using Gacha.Core.Data;

namespace Gacha.Core.Economy
{
    /// <summary>Idle loot per hour by highest cleared stage (economy.json "idle"): one growth factor per stage through the stage-30 and stage-600 rates.</summary>
    public sealed class IdleRates
    {
        /// <summary>Per resource: the stage-30 rate and the factor per stage that reaches the stage-600 rate.</summary>
        readonly Dictionary<string, (double at30, double factor)> _rates = new Dictionary<string, (double, double)>();
        public string[] Resources { get; private set; } = new string[0];
        public double CapHours { get; private set; } = 24;
        public double HourglassHours { get; private set; } = 2;

        public static IdleRates FromJson(string json)
        {
            var idle = Node.Of(Json.Parse(json)).Obj("idle");
            var r = new IdleRates { CapHours = idle.Num("capHours", 24), HourglassHours = idle.Num("hourglassHours", 2) };
            var rates = idle.Obj("rates");
            var names = new List<string>();
            foreach (var k in rates.Keys)
            {
                var n = rates.Obj(k);
                double at30 = n.Num("at30");
                r._rates[k] = (at30, at30 > 0 ? Math.Pow(n.Num("at600") / at30, 1.0 / 570) : 1);
                names.Add(k);
            }
            r.Resources = names.ToArray();
            return r;
        }

        public double PerHour(string resource, int highestStage)
        {
            if (highestStage <= 0 || !_rates.TryGetValue(resource, out var r)) return 0;
            return r.at30 * Math.Pow(r.factor, highestStage - 30);
        }
    }
}
