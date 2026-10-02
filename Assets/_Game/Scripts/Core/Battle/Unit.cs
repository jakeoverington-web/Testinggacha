using System;
using System.Collections.Generic;

namespace Gacha.Core.Battle
{
    public sealed class Status
    {
        public string Id;
        public double Remaining, V, Amount;   // V = magnitude; Amount = flat value fixed at apply time (thorns, burn per tick)
        public int Source;                    // unit index of the caster
        public List<string> Only;             // cc_immunity limited to some effects
        public double Delay;                  // scheduled start (Mireille's after-regen)
        public string Ab;                     // ability that applied it; effects it causes are credited to that ability
        public override string ToString() => $"{Id}({V:0.##},{Remaining:0.0}s)";
    }

    public sealed class Shield
    {
        public double Amount, Remaining, BreakHeal;
        public int Source;
    }

    /// <summary>A hero or summon on the field. All rule logic lives in Battle; this is state plus derived stats.</summary>
    public sealed class Unit
    {
        public int Index, Team;
        public HeroDef Def;                  // the hero; summons point at their owner's def for race/name
        public string Id;                    // hero id, or "wolf"/"stag"/"decoy"
        public bool IsSummon;
        public int Owner = -1;
        public SummonDef Summon;
        public Stats Base;
        public double Hp, Energy, X, Y, AttackTimer, CastLock, ExpiresAt = double.MaxValue;
        public bool Alive = true, Moved, NoRevive, Revived, Untargetable;
        public readonly double[] Cd = new double[2];
        public readonly List<Status> Statuses = new List<Status>();
        public readonly List<Shield> Shields = new List<Shield>();
        public readonly Dictionary<string, int> Counters = new Dictionary<string, int>();
        public readonly Dictionary<int, double> TriggerReadyAt = new Dictionary<int, double>();
        public double LastHeal;
        public double DrStart = -99; public int DrCount;
        public double[] HpHistory;           // ring buffer, one entry per tick
        public int HpHistoryPos;
        public double BonusHpPct, BonusAtkPct, BonusDefPct;   // team bonus
        /// <summary>Analysis switch: parts of the kit turned off ("ult", "s1", "s2", "passive", "basic").</summary>
        public HashSet<string> Off;
        /// <summary>Who this unit was attacking last tick (AI uses it to avoid wasting control on the focus target).</summary>
        public int LastTarget = -1;
        public bool IsOff(string part) => Off != null && Off.Contains(part);

        public string Name => IsSummon ? Def.Name + "'s " + Id : Def.Name;
        public bool IsHero => !IsSummon;
        public string Core => Def.Core;

        // ---------- statuses ----------
        public bool Has(string id) { for (int i = 0; i < Statuses.Count; i++) if (Statuses[i].Id == id && Statuses[i].Delay <= 0) return true; return false; }
        public Status Get(string id) { for (int i = 0; i < Statuses.Count; i++) if (Statuses[i].Id == id && Statuses[i].Delay <= 0) return Statuses[i]; return null; }
        public double Sum(string id) { double s = 0; for (int i = 0; i < Statuses.Count; i++) if (Statuses[i].Id == id && Statuses[i].Delay <= 0) s += Statuses[i].V; return s; }
        public double Max(string id) { double s = 0; for (int i = 0; i < Statuses.Count; i++) if (Statuses[i].Id == id && Statuses[i].Delay <= 0 && Statuses[i].V > s) s = Statuses[i].V; return s; }
        public int Count(string id) { int c = 0; for (int i = 0; i < Statuses.Count; i++) if (Statuses[i].Id == id && Statuses[i].Delay <= 0) c++; return c; }

        public double ShieldTotal { get { double s = 0; foreach (var x in Shields) s += x.Amount; return s; } }

        // ---------- derived stats (base x team bonus x buffs) ----------
        public double MaxHp => Base.Hp * (1 + BonusHpPct) * (1 + Max("maxhp_up") + (Has("dragonform") ? Get("dragonform").V : 0));
        public double Atk => Math.Max(0, Base.Atk * (1 + BonusAtkPct) * (1 + Max("atk_up") - Max("atk_down")));
        public double DefStat => Math.Max(0, Base.Def * (1 + BonusDefPct) * (1 + Max("def_up") - Max("def_down")));
        public double HpPct => MaxHp <= 0 ? 0 : Hp / MaxHp;
        public bool IsRanged => Base.Range > 2;

        public override string ToString() => $"{Name}#{Index}(T{Team} {Hp:0}/{MaxHp:0} e{Energy:0})";
    }

    public enum Ev { Start, Cast, Damage, Miss, Absorb, Heal, ShieldGain, StatusOn, Resist, StatusOff, Energy, Death, Revive, Summon, Cleanse, Dispel, Move, Cooldown, Zone, End }

    public struct BattleEvent
    {
        public double T;
        public Ev Type;
        public int Src, Dst;
        public string What;      // ability key, status id, damage kind
        public string Ab;        // which ability caused it: ult, s1, s2, basic, passive, dot
        public double Amount;
        public double V;         // extra value: status magnitude, zone/summon duration
        public bool Crit;
        public override string ToString() => $"{T:0.0} [{Ab}] {Type} {Src}->{Dst} {What} {Amount:0.#}{(Crit ? " CRIT" : "")}";
    }

    public sealed class BattleResult
    {
        public int Winner;             // 0 = team A, 1 = team B, -1 = timeout (attacker loses)
        public double Time;
        public ulong Hash;             // event-log fingerprint for golden replays
        public List<BattleEvent> Log;
    }
}
