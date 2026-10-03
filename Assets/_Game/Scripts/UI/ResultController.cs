using System.Linq;
using Gacha.Core.Campaign;
using UnityEngine.UIElements;

namespace Gacha.UI
{
    /// <summary>
    /// Result panel: win or loss, first-clear rewards and the new idle rate. With auto mode on (row 21) the next stage
    /// starts after 1.5 s; it stops on a loss or after the last stage. Arg: the FightResult.
    /// </summary>
    public sealed class ResultController : IScreen
    {
        const float AutoDelay = 1.5f;

        ScreenRouter _r;
        VisualElement _root;
        FightResult _result;
        float _autoIn = -1;

        public void Bind(VisualElement root, ScreenRouter router, object arg)
        {
            _r = router; _root = root; _result = (FightResult)arg;
            var s = _r.Data.Stages[_result.StageIndex - 1];
            var outcome = root.Q<Label>("outcome");
            outcome.text = _result.Won ? "Victory" : "Defeat";
            outcome.AddToClassList(_result.Won ? "outcome--win" : "outcome--loss");
            root.Q<Label>("stage").text = $"Stage {s.Chapter}-{s.Stage}  ·  {_result.Battle.Time:0}s";
            root.Q<Label>("rewards").text = _result.FirstClear
                ? "First clear: " + string.Join(" · ", s.FirstClear.Select(kv => $"{Placeholder.Short(kv.Value)} {Name(kv.Key)}"))
                : _result.Won ? "Already cleared: no first-clear reward." : "Swap heroes on the pre-battle screen and try again.";
            int h = _r.State.HighestCleared;
            root.Q<Label>("idle").text = h > 0 ? $"Idle chest: {Placeholder.Short(_r.Data.Idle.PerHour("gold", h))} gold/h" : "";

            bool hasNext = _result.Won && _result.StageIndex < _r.Data.Stages.Count;
            var next = root.Q<Button>("next");
            next.SetEnabled(hasNext);
            next.clicked += () => _r.Show("prebattle", _result.StageIndex + 1);
            root.Q<Button>("retry").clicked += () => _r.Show("prebattle", _result.StageIndex);
            root.Q<Button>("map").clicked += () => _r.Show("map");
            var stop = root.Q<Button>("stop-auto");
            stop.clicked += () => { _r.State.Auto = false; _r.Game.Save(); _autoIn = -1; ShowAuto(); };

            if (_r.State.AutoContinues(_r.Data, _result)) _autoIn = AutoDelay;
            ShowAuto();

            // After-fight statistics: opened by tap; your team first, switchable to the enemy team.
            var panel = root.Q("stats-panel");
            root.Q<Button>("stats").clicked += () => { _autoIn = -1; ShowAuto(); panel.AddToClassList("stats-panel--open"); ShowStats(0); };
            root.Q<Button>("stats-close").clicked += () => panel.RemoveFromClassList("stats-panel--open");
            root.Q<Button>("stats-you").clicked += () => ShowStats(0);
            root.Q<Button>("stats-enemy").clicked += () => ShowStats(1);
        }

        public void Tick(float dt)
        {
            if (_autoIn < 0) return;
            _autoIn -= dt;
            if (_autoIn <= 0)
            {
                _autoIn = -1;
                _r.Fight(_result.StageIndex + 1, _r.State.LastSetup[CampaignState.Mode], manual: false);
                return;
            }
            ShowAuto();
        }

        void ShowAuto()
        {
            bool running = _autoIn >= 0;
            _root.Q<Label>("auto").text = running ? $"Auto: next stage in {_autoIn:0.0} s" : _r.State.Auto && !_result.Won && !_result.Manual ? "Auto stopped after a defeat." : "";
            _root.Q<Button>("stop-auto").style.display = running ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>One card per hero in field order: race-coloured icon (greyed with "Died" if she fell) and three bars,
        /// each scaled to the highest value of that stat among the team shown.</summary>
        void ShowStats(int team)
        {
            _root.Q<Button>("stats-you").EnableInClassList("tab--on", team == 0);
            _root.Q<Button>("stats-enemy").EnableInClassList("tab--on", team == 1);
            var rows = _root.Q<ScrollView>("stats-rows");
            rows.Clear();
            var lines = _result.Stats.Where(l => l.Team == team).ToList();
            double maxDealt = System.Math.Max(1, lines.Max(l => (double?)l.Dealt) ?? 0);
            double maxHeal = System.Math.Max(1, lines.Max(l => (double?)l.Healed) ?? 0);
            double maxTaken = System.Math.Max(1, lines.Max(l => (double?)l.Taken) ?? 0);
            foreach (var l in lines)
            {
                var h = _r.Data.Heroes[l.Hero];
                var row = new VisualElement(); row.AddToClassList("stat-hero");
                if (!l.Alive) row.AddToClassList("stat-hero--fallen");
                var icon = new VisualElement(); icon.AddToClassList("stat-hero__icon");
                var colour = Placeholder.RaceColour(h.Core);
                // A fallen hero: grey the colour behind her, keep her name and "Died" bright.
                icon.style.backgroundColor = l.Alive ? colour : UnityEngine.Color.Lerp(colour, new UnityEngine.Color(0.22f, 0.22f, 0.24f), 0.7f);
                icon.Add(Placeholder.Label((team == 1 ? "Hollow " : "") + h.Name, "stat-hero__name"));
                icon.Add(Placeholder.Label("Died", "stat-hero__died"));
                row.Add(icon);
                var bars = new VisualElement(); bars.AddToClassList("stat-hero__bars");
                bars.Add(Bar(l.Dealt, maxDealt, "dealt"));
                bars.Add(Bar(l.Healed, maxHeal, "heal"));
                bars.Add(Bar(l.Taken, maxTaken, "taken"));
                row.Add(bars);
                rows.Add(row);
            }
        }

        static VisualElement Bar(double value, double max, string kind)
        {
            var bar = new VisualElement(); bar.AddToClassList("stat-bar");
            var track = new VisualElement(); track.AddToClassList("stat-bar__track");
            var fill = new VisualElement(); fill.AddToClassList("stat-bar__fill"); fill.AddToClassList("stat-bar__fill--" + kind);
            fill.style.width = Length.Percent((float)(100 * value / max));
            track.Add(fill); bar.Add(track);
            bar.Add(Placeholder.Label(Placeholder.Short(System.Math.Round(value)), "stat-bar__value"));
            return bar;
        }

        static string Name(string res) => res switch { "heroXp" => "Hero XP", "gold" => "gold", "starlight" => "Starlight", _ => res };
    }
}
