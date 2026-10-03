using System;
using System.Collections.Generic;
using Gacha.Core.Battle;
using Gacha.Core.Economy;

namespace Gacha.Core.Progression
{
    /// <summary>
    /// Star ranks (decisions rows 2, 3, 5, 31, 32; phase 2 spec). Each rank costs copies, fodder and gold (progression.json).
    /// Fodder = a spare 1-star hero of the same race (her spare copy first, else the hero herself) or a Race Sigil of that race.
    /// Heroes in a Contract slot, starred heroes and locked heroes (e.g. wearing gear) cannot be fed.
    /// Reset is free and refunds copies, gold, and the fodder as Sigils, so value is never lost or gained.
    /// </summary>
    public static class Stars
    {
        /// <summary>Why the hero cannot go up a star with this fodder, or null if she can.</summary>
        public static string Why(GameData g, Collection c, string hero, IList<string> fodderHeroes, int sigils, Wallet w, Func<string, bool> locked = null)
        {
            var h = c.Get(hero);
            if (h == null) return "not owned";
            if (h.Stars >= g.Levels.MaxStars) return "already at max stars";
            var row = g.StarCosts.For(h.Stars + 1);
            if (h.Copies < row.Copies) return $"needs {row.Copies} spare copies";
            if (w.Get("gold") < row.Gold) return $"needs {row.Gold:N0} gold";
            if (fodderHeroes.Count + sigils != row.Fodder) return $"needs exactly {row.Fodder} fodder";
            string race = g.Heroes[hero].Core;
            if (sigils < 0 || c.SigilsOf(race) < sigils) return "not enough Sigils";
            var uses = new Dictionary<string, int>();
            foreach (var f in fodderHeroes) uses[f] = (uses.TryGetValue(f, out var n) ? n : 0) + 1;
            foreach (var kv in uses)
            {
                var fh = c.Get(kv.Key);
                if (fh == null) return "fodder not owned: " + kv.Key;
                if (kv.Key == hero) return "a hero cannot feed herself";
                if (g.Heroes[kv.Key].Core != race) return "fodder must be the same race";
                if (fh.Stars != 1) return "reset a starred hero before feeding her";
                if (c.InSlot(kv.Key)) return "a Contract hero cannot be fed";
                if (locked != null && locked(kv.Key)) return "remove her gear first";
                if (kv.Value > fh.Copies + 1) return "not enough copies of " + kv.Key;
            }
            return null;
        }

        public static bool Raise(GameData g, Collection c, string hero, IList<string> fodderHeroes, int sigils, Wallet w, Func<string, bool> locked = null)
        {
            if (Why(g, c, hero, fodderHeroes, sigils, w, locked) != null) return false;
            var h = c.Get(hero);
            var row = g.StarCosts.For(h.Stars + 1);
            h.Copies -= row.Copies;
            w.Add("gold", -row.Gold);
            c.AddSigils(g.Heroes[hero].Core, -sigils);
            foreach (var f in fodderHeroes)
            {
                var fh = c.Get(f);
                if (fh.Copies > 0) fh.Copies--; else c.Heroes.Remove(f);
            }
            h.Stars++;
            return true;
        }

        /// <summary>
        /// Picks fodder for the next rank: Sigils first, then spare copies of same-race heroes, then free 1-star same-race
        /// heroes (not in a slot, not locked). Returns fewer than needed when the player is short.
        /// </summary>
        public static (List<string> heroes, int sigils) AutoFodder(GameData g, Collection c, string hero, Func<string, bool> locked = null)
        {
            var heroes = new List<string>();
            var h = c.Get(hero);
            if (h == null || h.Stars >= g.Levels.MaxStars) return (heroes, 0);
            int need = g.StarCosts.For(h.Stars + 1).Fodder;
            string race = g.Heroes[hero].Core;
            int sigils = Math.Min(need, c.SigilsOf(race));
            need -= sigils;
            var candidates = new List<OwnedHero>();
            foreach (var id in g.HeroOrder)
            {
                var o = c.Get(id);
                if (o == null || id == hero || g.Heroes[id].Core != race) continue;
                candidates.Add(o);
            }
            foreach (var o in candidates)
                for (int k = 0; k < o.Copies && need > 0; k++) { heroes.Add(o.Id); need--; }
            foreach (var o in candidates)
            {
                if (need <= 0) break;
                if (o.Stars != 1 || c.InSlot(o.Id) || (locked != null && locked(o.Id))) continue;
                heroes.Add(o.Id); need--;
            }
            return (heroes, sigils);
        }

        /// <summary>Back to 1 star with a full refund: copies, gold, and the fodder spent as Sigils of her race.</summary>
        public static void Reset(GameData g, Collection c, string hero, Wallet w)
        {
            var h = c.Get(hero);
            if (h == null) return;
            int copies = 0, fodder = 0; long gold = 0;
            for (int s = 2; s <= h.Stars; s++) { var r = g.StarCosts.For(s); copies += r.Copies; fodder += r.Fodder; gold += r.Gold; }
            h.Copies += copies;
            w.Add("gold", gold);
            c.AddSigils(g.Heroes[hero].Core, fodder);
            h.Stars = 1;
        }
    }
}
