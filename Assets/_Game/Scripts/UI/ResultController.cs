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

        static string Name(string res) => res switch { "heroXp" => "Hero XP", "gold" => "gold", "starlight" => "Starlight", _ => res };
    }
}
