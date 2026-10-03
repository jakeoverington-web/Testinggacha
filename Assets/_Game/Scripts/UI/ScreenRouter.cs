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
        readonly VisualElement _host, _tabs;
        readonly UiRoot _ui;
        IScreen _current;
        string _screen;

        public ScreenRouter(VisualElement host, UiRoot ui, VisualElement tabs = null)
        {
            _host = host; _ui = ui; _tabs = tabs;
            if (_tabs == null) return;
            foreach (var (id, label) in new[] { ("map", "Campaign"), ("heroes", "Heroes") })
            {
                var b = new Button(() => Show(id)) { text = label, name = "tab-" + id };
                b.AddToClassList("tabbar__tab");
                _tabs.Add(b);
            }
        }

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
                case "heroes": tree = _ui.Heroes; ctl = new HeroesController(); break;
                case "herodetail": tree = _ui.HeroDetail; ctl = new HeroDetailController(); break;
                case "dev": tree = _ui.Dev; ctl = new DevPanelController(); break;
                default: tree = _ui.Map; ctl = new CampaignMapController(); break;
            }
            _screen = screen;
            if (_tabs != null)
            {
                bool show = screen == "map" || screen == "heroes" || screen == "herodetail";
                _tabs.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
                foreach (var t in _tabs.Children())
                    t.EnableInClassList("tabbar__tab--on", t.name == "tab-" + (screen == "herodetail" ? "heroes" : screen == "heroes" ? "heroes" : "map"));
            }
            _host.Clear();
            var root = tree.Instantiate();
            root.AddToClassList("screen");
            _host.Add(root);
            _current = ctl;
            ctl.Bind(root, this, arg);
        }

        public void Tick(float dt) => _current?.Tick(dt);

        /// <summary>Starts a live fight (manual = tap-to-cast ultimates), saves, and shows the battle; it finishes itself.</summary>
        public void Fight(int stageIndex, IList<string> slots, bool manual)
        {
            var session = State.StartFight(Data, stageIndex, slots, manual);
            Game.Save();
            Show("battle", session);
        }

        /// <summary>Applies a finished (or skipped) fight, saves, and shows the result.</summary>
        public void Finish(FightSession session)
        {
            var r = State.FinishFight(Data, session, Game.Now);
            Game.Save();
            Show("result", r);
        }
    }
}
