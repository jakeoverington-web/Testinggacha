using System.Collections.Generic;

namespace Gacha.Core.Battle
{
    public enum InputKind { CastUltimate, SetAuto }

    /// <summary>
    /// A player input during a live battle (redesign spec). Queued inputs apply at the start of the next tick;
    /// applied inputs are logged with that tick so seed + setups + log replays the same battle (hard rule 4).
    /// </summary>
    public struct BattleInput
    {
        public int Tick;        // set when applied
        public InputKind Kind;
        public int Unit;        // CastUltimate: the hero's unit index
        public int Team;        // SetAuto
        public bool On;         // SetAuto: true = ultimates cast themselves
    }

    public sealed partial class Battle
    {
        /// <summary>Per team: true = full-energy heroes wait for a CastUltimate input instead of casting.</summary>
        public readonly bool[] ManualUltimates = new bool[2];

        /// <summary>Inputs applied so far, in order, each stamped with its tick.</summary>
        public readonly List<BattleInput> Inputs = new List<BattleInput>();

        readonly List<BattleInput> _queued = new List<BattleInput>();

        public int TickCount => _tickCount;

        public void Queue(BattleInput input) => _queued.Add(input);

        void ApplyInputs()
        {
            foreach (var input in _queued)
            {
                var i = input;
                i.Tick = _tickCount;
                if (i.Kind == InputKind.SetAuto)
                {
                    if (i.Team < 0 || i.Team > 1) continue;
                    ManualUltimates[i.Team] = !i.On;
                }
                else
                {
                    if (i.Unit < 0 || i.Unit >= Units.Count) continue;
                    var u = Units[i.Unit];
                    if (!u.Alive || !u.IsHero || !ManualUltimates[u.Team]) continue;
                    u.UltRequested = true;
                }
                Inputs.Add(i);
            }
            _queued.Clear();
        }

        /// <summary>Re-runs a battle from its seed, setups, starting manual flags and input log.</summary>
        public static BattleResult Replay(GameData g, TeamSetup a, TeamSetup b, ulong seed, bool[] manual, IList<BattleInput> inputs)
        {
            var battle = new Battle(g, a, b, seed);
            battle.ManualUltimates[0] = manual[0]; battle.ManualUltimates[1] = manual[1];
            int next = 0;
            while (!battle.Over)
            {
                while (next < inputs.Count && inputs[next].Tick == battle._tickCount + 1) battle.Queue(inputs[next++]);
                battle.Step();
            }
            return battle.Result();
        }
    }
}
