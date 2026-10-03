using System.Linq;
using Gacha.Core.Progression;
using UnityEngine.UIElements;

namespace Gacha.UI
{
    /// <summary>
    /// Heroes tab (phase 2 spec; Omniheroes reference): the 5 Contract slots on top with level and power, then the owned
    /// collection in a grid with race tabs and a count. Tap a hero for her detail screen.
    /// </summary>
    public sealed class HeroesController : IScreen
    {
        static readonly (string id, string label)[] Races =
            { ("", "All"), ("high", "Lumarin"), ("dark", "Noctyr"), ("nature", "Verdani"), ("ocean", "Thalyri"), ("arcane", "Aethari") };

        ScreenRouter _r;
        VisualElement _root;
        string _race = "";

        public void Bind(VisualElement root, ScreenRouter router, object arg)
        {
            _r = router; _root = root;
            var dev = root.Q<Button>("dev");
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            dev.clicked += () => _r.Show("dev");
#else
            dev.style.display = DisplayStyle.None;
#endif
            var tabs = root.Q("tabs");
            foreach (var (id, label) in Races)
            {
                var race = id;
                var t = new Button(() => { _race = race; Fill(); }) { text = label };
                t.AddToClassList("tab"); t.userData = race;
                tabs.Add(t);
            }
            Fill();
        }

        public void Tick(float dt) { }

        void Fill()
        {
            var g = _r.Data; var c = _r.State.Collection;
            var slots = _root.Q("slots");
            slots.Clear();
            for (int i = 0; i < Collection.SlotCount; i++)
            {
                var s = c.Slots[i];
                if (s.Hero == "")
                {
                    var empty = new VisualElement(); empty.AddToClassList("card"); empty.AddToClassList("card--slot"); empty.AddToClassList("card--empty");
                    empty.Add(Placeholder.Label($"Empty\nLv {s.Level}", "card__meta"));
                    slots.Add(empty);
                    continue;
                }
                var card = Card(s.Hero);
                card.AddToClassList("card--slot");
                slots.Add(card);
            }

            int owned = c.Heroes.Count;
            _root.Q<Label>("count").text = $"Collection  {owned} / {g.HeroOrder.Count}   ·   everyone else is level {c.SyncLevel}";
            foreach (var t in _root.Q("tabs").Children()) t.EnableInClassList("tab--on", (string)t.userData == _race);

            var grid = _root.Q<ScrollView>("grid");
            grid.Clear();
            foreach (var id in g.HeroOrder.Where(c.Owns))
            {
                if (_race != "" && g.Heroes[id].Core != _race) continue;
                grid.Add(Card(id));
            }
        }

        VisualElement Card(string id)
        {
            var g = _r.Data; var c = _r.State.Collection; var h = g.Heroes[id]; var o = c.Get(id);
            var card = new VisualElement();
            card.AddToClassList("card");
            card.style.backgroundColor = Placeholder.RaceColour(h.Core);
            if (o.Stars >= 10) card.AddToClassList("card--diamond");
            card.Add(Placeholder.Label(h.Name, "card__name"));
            var stars = Placeholder.Label(Placeholder.Stars(o.Stars), "card__stars");
            if (o.Stars > 5) stars.AddToClassList("card__stars--diamond");
            card.Add(stars);
            card.Add(Placeholder.Label($"{h.Role} · Lv {c.Level(g, id)}\nPower {Placeholder.Short(_r.State.HeroPower(g, id))}" + (o.Copies > 0 ? $"\n+{o.Copies} copies" : ""), "card__meta"));
            card.RegisterCallback<ClickEvent>(_ => _r.Show("herodetail", id));
            return card;
        }
    }
}
