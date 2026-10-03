using System.Collections.Generic;
using System.Linq;
using Gacha.Core.Campaign;
using UnityEngine.UIElements;

namespace Gacha.UI
{
    /// <summary>
    /// Pre-battle on the battlefield (redesign spec): both teams stand at their start cells; tap one of your heroes then
    /// another slot to swap; a roster strip with race tabs adds and removes heroes; Quick Deploy; Auto-Battle or Battle.
    /// Arg: the stage index.
    /// </summary>
    public sealed class PreBattleController : IScreen
    {
        static readonly (string id, string label)[] Races =
            { ("", "All"), ("high", "Lumarin"), ("dark", "Noctyr"), ("nature", "Verdani"), ("ocean", "Thalyri"), ("arcane", "Aethari") };

        ScreenRouter _r;
        VisualElement _root, _field;
        int _stage, _level, _stars, _picked = -1;
        string _race = "";
        List<string> _slots;

        public void Bind(VisualElement root, ScreenRouter router, object arg)
        {
            _r = router; _root = root; _stage = (int)arg;
            var s = _r.Data.Stages[_stage - 1];
            _level = Expected.Level(_stage); _stars = Expected.Stars(s.Chapter);
            _field = root.Q("field");
            root.Q<Label>("title").text = $"Stage {s.Chapter}-{s.Stage}";
            root.Q<Label>("level-note").text = "Your heroes fight at their own levels, stars and gear";
            root.Q<Button>("back").clicked += () => _r.Show("map", s.Chapter);
            root.Q<Button>("quick").clicked += () => { _slots = QuickDeploy.Pick(Owned(), h => _r.State.HeroPower(_r.Data, h), _r.Data); _picked = -1; Refresh(); };
            root.Q<Button>("auto").clicked += () => Go(manual: false);
            root.Q<Button>("manual").clicked += () => Go(manual: true);

            _r.State.LastSetup.TryGetValue(CampaignState.Mode, out var last);
            _slots = CampaignState.Slots(last?.Select(h => _r.State.Collection.Owns(h) ? h : "").ToList());
            if (_slots.All(h => h == "")) _slots = QuickDeploy.Pick(Owned(), h => _r.State.HeroPower(_r.Data, h), _r.Data);

            var tabs = root.Q("tabs");
            foreach (var (id, label) in Races)
            {
                var race = id;
                var t = new Button(() => { _race = race; Refresh(); }) { text = label };
                t.AddToClassList("tab"); t.userData = race;
                tabs.Add(t);
            }
            Refresh();
        }

        public void Tick(float dt) { }

        /// <summary>Owned heroes in roster order.</summary>
        List<string> Owned() => _r.Data.HeroOrder.Where(_r.State.Collection.Owns).ToList();

        void Go(bool manual)
        {
            if (_slots.Any(h => h != "")) _r.Fight(_stage, _slots.ToList(), manual);
        }

        void TapSlot(int i)
        {
            if (_picked < 0) { if (_slots[i] != "") _picked = i; }
            else if (_picked == i) _picked = -1;
            else { (_slots[_picked], _slots[i]) = (_slots[i], _slots[_picked]); _picked = -1; }
            Refresh();
        }

        void TapRoster(string id)
        {
            int at = _slots.IndexOf(id);
            if (at >= 0) _slots[at] = "";
            else
            {
                int empty = _slots.IndexOf("");
                if (empty < 0) return;
                _slots[empty] = id;
            }
            _picked = -1;
            Refresh();
        }

        void Refresh()
        {
            var g = _r.Data; var s = g.Stages[_stage - 1];
            var team = _slots.Where(h => h != "").ToList();
            _root.Q<Label>("power-you").text = Placeholder.Short(team.Sum(h => _r.State.HeroPower(g, h)));
            _root.Q<Label>("power-enemy").text = Placeholder.Short(s.Power);

            _field.Clear();
            var cells = g.Tuning.Formation;
            for (int i = 0; i < CampaignState.MaxTeam; i++)
            {
                int slot = i;
                VisualElement el;
                if (_slots[i] == "")
                {
                    el = new VisualElement(); el.AddToClassList("slot"); el.AddToClassList("slot--empty");
                    el.Add(Placeholder.Label("+", "slot__plus"));
                }
                else el = Token(g.Heroes[_slots[i]], $"Lv {_r.State.Collection.Level(g, _slots[i])}", hollow: false);
                if (i == _picked) el.AddToClassList("slot--picked");
                Place(el, cells[i], ally: true);
                el.RegisterCallback<ClickEvent>(_ => TapSlot(slot));
                _field.Add(el);
            }
            for (int i = 0; i < s.Enemies.Count; i++)
            {
                var e = s.Enemies[i];
                var el = Token(g.Heroes[e.Hero], $"Lv {e.Level}", hollow: true);
                el.AddToClassList("slot--enemy");
                Place(el, cells[i % cells.Count], ally: false);
                _field.Add(el);
            }

            foreach (var t in _root.Q("tabs").Children()) t.EnableInClassList("tab--on", (string)t.userData == _race);

            var roster = _root.Q<ScrollView>("roster");
            float y = roster.scrollOffset.y;
            roster.Clear();
            foreach (var id in Owned())
            {
                var h = g.Heroes[id];
                if (_race != "" && h.Core != _race) continue;
                var card = Placeholder.HeroTile(h, null);
                if (_slots.Contains(id)) card.AddToClassList("tile--selected");
                card.RegisterCallback<ClickEvent>(_ => TapRoster(id));
                roster.Add(card);
            }
            roster.schedule.Execute(() => roster.scrollOffset = new UnityEngine.Vector2(0, y));

            bool any = team.Count > 0;
            _root.Q<Button>("auto").SetEnabled(any);
            _root.Q<Button>("manual").SetEnabled(any);
        }

        static VisualElement Token(Gacha.Core.Battle.HeroDef h, string meta, bool hollow)
        {
            var el = new VisualElement();
            el.AddToClassList("slot");
            el.style.backgroundColor = Placeholder.RaceColour(h.Core);
            el.Add(Placeholder.Label((hollow ? "Hollow " : "") + h.Name, "slot__name"));
            el.Add(Placeholder.Label($"{h.Role} · {meta}", "slot__meta"));
            return el;
        }

        /// <summary>Same field mapping as the battle view: battle.json colX/rowY, allies mirrored to the left.</summary>
        void Place(VisualElement el, int[] cell, bool ally)
        {
            var t = _r.Data.Tuning;
            double x = (ally ? -1 : 1) * t.ColX[System.Math.Min(cell[0], t.ColX.Length - 1)];
            double y = t.RowY[System.Math.Min(cell[1], t.RowY.Length - 1)];
            el.style.left = Length.Percent((float)((x + t.HalfWidth) / (2 * t.HalfWidth) * 100));
            el.style.top = Length.Percent((float)((y + t.HalfDepth) / (2 * t.HalfDepth) * 80 + 10));
        }
    }
}
