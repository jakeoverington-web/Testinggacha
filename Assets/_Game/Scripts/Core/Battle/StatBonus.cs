using System.Collections.Generic;

namespace Gacha.Core.Battle
{
    /// <summary>
    /// Extra stats a hero brings into battle (gear, set bonuses; phase 2 spec). Pct fields multiply the hero's stat;
    /// the rest add to it. ShieldRecv multiplies shields she receives; StartEnergy is her energy when the battle starts.
    /// Field names match gear.json stat names (camelCase).
    /// </summary>
    public sealed class StatBonus
    {
        public double HpPct, AtkPct, DefPct, AtkSpdPct, MoveSpdPct, Haste, CritRate, CritDmg, Accuracy, Dodge, Block,
            EffectHit, EffectRes, Lifesteal, HealPower, HealRecv, EnergyRegen, ShieldRecv, StartEnergy;

        public void ApplyTo(Stats s)
        {
            s.Hp *= 1 + HpPct; s.Atk *= 1 + AtkPct; s.Def *= 1 + DefPct;
            s.AtkSpd *= 1 + AtkSpdPct; s.MoveSpd *= 1 + MoveSpdPct;
            s.Haste += Haste; s.CritRate += CritRate; s.CritDmg += CritDmg; s.Accuracy += Accuracy; s.Dodge += Dodge; s.Block += Block;
            s.EffectHit += EffectHit; s.EffectRes += EffectRes; s.Lifesteal += Lifesteal; s.HealPower += HealPower; s.HealRecv += HealRecv;
            s.EnergyRegen += EnergyRegen;
        }

        /// <summary>Adds a named stat (gear.json names); unknown names are ignored.</summary>
        public void Add(string stat, double v)
        {
            switch (stat)
            {
                case "hpPct": HpPct += v; break;
                case "atkPct": AtkPct += v; break;
                case "defPct": DefPct += v; break;
                case "atkSpdPct": AtkSpdPct += v; break;
                case "moveSpdPct": MoveSpdPct += v; break;
                case "haste": Haste += v; break;
                case "critRate": CritRate += v; break;
                case "critDmg": CritDmg += v; break;
                case "accuracy": Accuracy += v; break;
                case "dodge": Dodge += v; break;
                case "block": Block += v; break;
                case "effectHit": EffectHit += v; break;
                case "effectRes": EffectRes += v; break;
                case "lifesteal": Lifesteal += v; break;
                case "healPower": HealPower += v; break;
                case "healRecv": HealRecv += v; break;
                case "energyRegen": EnergyRegen += v; break;
                case "shieldRecv": ShieldRecv += v; break;
                case "startEnergy": StartEnergy += v; break;
            }
        }

        public void Add(IDictionary<string, double> stats) { foreach (var kv in stats) Add(kv.Key, kv.Value); }
    }
}
