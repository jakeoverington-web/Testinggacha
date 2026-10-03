using System.Linq;
using Gacha.Core;
using Gacha.Core.Gear;
using UnityEngine.UIElements;

namespace Gacha.UI
{
    /// <summary>
    /// Developer-only grants for testing progression before summoning exists (phase 2 spec; owner, 2026-10-03).
    /// Opened only from the Heroes tab's Dev button, which exists only in the editor and development builds.
    /// </summary>
    public sealed class DevPanelController : IScreen
    {
        ScreenRouter _r;
        VisualElement _root;

        public void Bind(VisualElement root, ScreenRouter router, object arg)
        {
            _r = router; _root = root;
            root.Q<Button>("back").clicked += () => _r.Show("heroes");
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var s = _r.State; var g = _r.Data;
            var rng = new Rng((ulong)System.DateTime.UtcNow.Ticks);   // dev convenience only, never part of game rules
            Add("+1M gold", () => s.Wallet.Add("gold", 1e6));
            Add("+1M Hero XP", () => s.Wallet.Add("heroXp", 1e6));
            Add("+10K Starlight", () => s.Wallet.Add("starlight", 1e4));
            Add("+1 random new hero", () =>
            {
                var missing = g.HeroOrder.Where(h => !s.Collection.Owns(h)).ToList();
                if (missing.Count > 0) s.Collection.Add(missing[rng.Range(0, missing.Count)]);
            });
            Add("+5 copies of every owned hero", () => { foreach (var h in s.Collection.Heroes.Keys.ToList()) s.Collection.Add(h, 5); });
            Add("+5 Sigils of each race", () => { foreach (var race in new[] { "high", "dark", "nature", "ocean", "arcane" }) s.Collection.AddSigils(race, 5); });
            Add("+5 random gear", () =>
            {
                int unlocked = GearDrops.UnlockedRarities(g.Gear, s.HighestCleared);
                for (int i = 0; i < 5; i++) GearDrops.Roll(rng, g.Gear, unlocked, s.Inventory);
            });
            Add("+5 Legendary gear", () =>
            {
                for (int i = 0; i < 5; i++)
                    s.Inventory.Add(g.Gear.Types[rng.Range(0, g.Gear.Types.Count)].Id, g.Gear.Sets[rng.Range(0, g.Gear.Sets.Count)].Id, 4);
            });
#endif
            Refresh("");
        }

        public void Tick(float dt) { }

        void Add(string label, System.Action grant)
        {
            var b = new Button(() => { grant(); _r.Game.Save(); Refresh("Granted: " + label); }) { text = label };
            b.AddToClassList("btn");
            _root.Q("buttons").Add(b);
        }

        void Refresh(string message)
        {
            var s = _r.State;
            _root.Q<Label>("wallet").text = $"Gold {Placeholder.Short(s.Wallet.Get("gold"))} · Hero XP {Placeholder.Short(s.Wallet.Get("heroXp"))} · " +
                                            $"Starlight {Placeholder.Short(s.Wallet.Get("starlight"))} · Heroes {s.Collection.Heroes.Count} · Gear {s.Inventory.Items.Count}";
            _root.Q<Label>("message").text = message;
        }
    }
}
