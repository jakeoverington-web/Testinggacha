using System.Collections.Generic;
using Gacha.Core.Battle;
using Gacha.Core.Campaign;
using Gacha.Game;
using UnityEngine.UIElements;

namespace Gacha.UI
{
    /// <summary>One screen's controller: fills its UXML tree and reacts to taps.</summary>
    public interface IScreen
    {
        void Bind(VisualElement root, ScreenRouter router, object arg);
        void Tick(float dt);
    }

    /// <summary>Shows one screen at a time inside the host element: "map", "prebattle" (stage index), "battle" and "result" (FightResult).</summary>
    public sealed class ScreenRouter
    {
        readonly VisualElement _host;
        readonly UiRoot _ui;
        IScreen _current;

        public ScreenRouter(VisualElement host, UiRoot ui) { _host = host; _ui = ui; }

        public GameService Game => GameService.Instance;
        public GameData Data => Game.Data;
        public CampaignState State => Game.State;

        public void Show(string screen, object arg = null)
        {
            VisualTreeAsset tree; IScreen ctl;
            switch (screen)
            {
                case "prebattle": tree = _ui.PreBattle; ctl = new PreBattleController(); break;
                case "battle": tree = _ui.Battle; ctl = new BattleViewController(); break;
                case "result": tree = _ui.Result; ctl = new ResultController(); break;
                default: tree = _ui.Map; ctl = new CampaignMapController(); break;
            }
            _host.Clear();
            var root = tree.Instantiate();
            root.AddToClassList("screen");
            _host.Add(root);
            _current = ctl;
            ctl.Bind(root, this, arg);
        }

        public void Tick(float dt) => _current?.Tick(dt);

        /// <summary>Plays the stage in the sim, saves, then shows the replay.</summary>
        public void Fight(int stageIndex, IList<string> heroes)
        {
            var r = State.Fight(Data, stageIndex, heroes, Game.Now);
            Game.Save();
            Show("battle", r);
        }
    }
}
