using System.Collections.Generic;
using System.Linq;
using Gacha.Core.Campaign;
using UnityEngine.UIElements;

namespace Gacha.UI
{
    /// <summary>
    /// Pre-battle (decisions row 18): the enemy team, your team pre-filled from your last campaign team, a picker of
    /// all 60 heroes (phase 1: every hero is available) and Fight. Arg: the stage index.
    /// </summary>
    public sealed class PreBattleController : IScreen
    {
        static readonly string[] DefaultRoles = { "tank", "dps", "dps", "healer", "support" };

        ScreenRouter _r;
        VisualElement _root;
        int _stage, _level, _stars;
        readonly List<string> _team = new List<string>();

        public void Bind(VisualElement root, ScreenRouter router, object arg)
        {
            _r = router; _root = root; _stage = (int)arg;
            var s = _r.Data.Stages[_stage - 1];
            _level = Expected.Level(_stage); _stars = Expected.Stars(s.Chapter);
            root.Q<Label>("title").text = $"Stage {s.Chapter}-{s.Stage}";
            root.Q<Label>("your-label").text = $"Your team (fights at level {_level}, {Placeholder.Stars(_stars)})";
            root.Q<Button>("back").clicked += () => _r.Show("map", s.Chapter);
            root.Q<Button>("fight").clicked += () => { if (_team.Count > 0) _r.Fight(_stage, _team.ToList()); };

            var enemyRow = root.Q("enemy-row");
            foreach (var e in s.Enemies)
                enemyRow.Add(Placeholder.HeroTile(_r.Data.Heroes[e.Hero], $"Lv {e.Level} {Placeholder.Stars(e.Stars)}", hollow: true));

            if (_r.State.LastSetup.TryGetValue(CampaignState.Mode, out var last))
                _team.AddRange(last.Where(_r.Data.Heroes.ContainsKey).Take(CampaignState.MaxTeam));
            if (_team.Count == 0) _team.AddRange(DefaultTeam());
            Refresh();
        }

        public void Tick(float dt) { }

        IEnumerable<string> DefaultTeam()
        {
            var picked = new List<string>();
            foreach (var role in DefaultRoles)
            {
                var id = Roster().FirstOrDefault(h => _r.Data.Heroes[h].Role == role && !picked.Contains(h));
                if (id != null) picked.Add(id);
            }
            return picked;
        }

        /// <summary>Playable heroes: everything in heroes.json (test-only units never load in the game).</summary>
        IEnumerable<string> Roster() => _r.Data.HeroOrder;

        void Toggle(string id)
        {
            if (_team.Remove(id)) { Refresh(); return; }
            if (_team.Count >= CampaignState.MaxTeam) return;
            _team.Add(id);
            Refresh();
        }

        void Refresh()
        {
            var s = _r.Data.Stages[_stage - 1];
            long mine = _team.Count == 0 ? 0 : TeamPower.Of(_r.Data, _team, _level, _stars);
            _root.Q<Label>("power").text = $"Your power {Placeholder.Short(mine)}  ·  Recommended {Placeholder.Short(s.Power)}";

            var row = _root.Q("your-row");
            row.Clear();
            for (int i = 0; i < CampaignState.MaxTeam; i++)
            {
                if (i < _team.Count)
                {
                    string id = _team[i];
                    var t = Placeholder.HeroTile(_r.Data.Heroes[id], "tap to remove");
                    t.RegisterCallback<ClickEvent>(_ => Toggle(id));
                    row.Add(t);
                }
                else
                {
                    var empty = new VisualElement();
                    empty.AddToClassList("tile"); empty.AddToClassList("tile--empty");
                    row.Add(empty);
                }
            }

            var picker = _root.Q<ScrollView>("picker");
            float scroll = picker.scrollOffset.y;
            picker.Clear();
            foreach (var id in Roster())
            {
                var h = _r.Data.Heroes[id];
                var t = Placeholder.HeroTile(h, null);
                if (_team.Contains(id)) t.AddToClassList("tile--selected");
                t.RegisterCallback<ClickEvent>(_ => Toggle(id));
                picker.Add(t);
            }
            picker.schedule.Execute(() => picker.scrollOffset = new UnityEngine.Vector2(0, scroll));
            _root.Q<Button>("fight").SetEnabled(_team.Count > 0);
        }
    }
}
