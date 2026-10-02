using System;

namespace Gacha.Core
{
    /// <summary>
    /// The one source of randomness for game rules (hard rule 4). SplitMix64: tiny, fast, and identical on every platform,
    /// so a seed replays the same battle or pull everywhere. Never use System.Random or UnityEngine.Random in rules.
    /// </summary>
    public sealed class Rng
    {
        ulong _state;
        public Rng(ulong seed) { _state = seed; }

        public ulong NextULong()
        {
            ulong z = (_state += 0x9E3779B97F4A7C15UL);
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }

        /// <summary>Uniform in [0, 1).</summary>
        public double NextDouble() => (NextULong() >> 11) * (1.0 / (1UL << 53));

        /// <summary>Uniform int in [min, maxExclusive).</summary>
        public int Range(int min, int maxExclusive)
        {
            if (maxExclusive <= min) return min;
            return min + (int)(NextULong() % (ulong)(maxExclusive - min));
        }

        /// <summary>True with the given probability (0..1).</summary>
        public bool Chance(double p) => p > 0 && (p >= 1 || NextDouble() < p);

        /// <summary>A child generator for an independent stream (e.g. one per battle in a sweep).</summary>
        public Rng Fork() => new Rng(NextULong());
    }
}
