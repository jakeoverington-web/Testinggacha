using System.Collections.Generic;
using Gacha.Core.Battle;
using Gacha.Core.Campaign;
using UnityEngine.UIElements;

namespace Gacha.UI
{
    /// <summary>
    /// Battle playback (decisions rows 16, 19): re-runs the fight with the same seed and setups, a little each frame,
    /// and draws every unit as a flat-colour token at its X/Y (allies left, enemies right). Arg: the FightResult.
    /// </summary>
    public sealed class BattleViewController : IScreen
    {
        static readonly float[] Speeds = { 1f, 2f, 4f };

        ScreenRouter _r;
        VisualElement _root, _field;
        FightResult _result;
        Battle _sim;
        int _speed;
        float _endDelay = 0.8f;
        /// <summary>Real time not yet played: the sim only moves in whole ticks (0.1 s), and RunFor always runs at least one.</summary>
        double _pending;
        readonly Dictionary<int, (VisualElement token, VisualElement fill)> _tokens = new Dictionary<int, (VisualElement, VisualElement)>();

        public void Bind(VisualElement root, ScreenRouter router, object arg)
        {
            _r = router; _root = root; _result = (FightResult)arg;
            var s = _r.Data.Stages[_result.StageIndex - 1];
            root.Q<Label>("title").text = $"Stage {s.Chapter}-{s.Stage}";
            _field = root.Q("field");
            _sim = new Battle(_r.Data, _result.Player, _result.Enemy, _result.Seed) { KeepLog = false };
            var speed = root.Q<Button>("speed");
            speed.clicked += () => { _speed = (_speed + 1) % Speeds.Length; speed.text = $"Speed {Speeds[_speed]:0}×"; };
            root.Q<Button>("skip").clicked += Finish;
            Draw();
        }

        public void Tick(float dt)
        {
            if (_sim == null) return;
            if (!_sim.Over)
            {
                _pending += dt * Speeds[_speed];
                while (_pending >= _sim.T.Tick && !_sim.Over) { _sim.RunFor(_sim.T.Tick); _pending -= _sim.T.Tick; }
                Draw();
                return;
            }
            _endDelay -= dt;
            if (_endDelay <= 0) Finish();
        }

        void Finish()
        {
            if (_sim == null) return;
            _sim = null;
            _r.Show("result", _result);
        }

        void Draw()
        {
            var t = _sim.T;
            _root.Q<Label>("clock").text = $"{_sim.Time:0}s / {t.TimeLimit:0}s";
            foreach (var u in _sim.Units)
            {
                if (!_tokens.TryGetValue(u.Index, out var tk)) tk = _tokens[u.Index] = MakeToken(u);
                tk.token.style.left = Length.Percent((float)((u.X + t.HalfWidth) / (2 * t.HalfWidth) * 100));
                tk.token.style.top = Length.Percent((float)((u.Y + t.HalfDepth) / (2 * t.HalfDepth) * 80 + 10));
                tk.fill.style.width = Length.Percent((float)(System.Math.Max(0, u.HpPct) * 100));
                tk.token.EnableInClassList("token--dead", !u.Alive);
            }
        }

        (VisualElement, VisualElement) MakeToken(Unit u)
        {
            var token = new VisualElement();
            token.AddToClassList("token");
            if (u.Team == 1) token.AddToClassList("token--enemy");
            if (u.IsSummon) token.AddToClassList("token--summon");
            token.style.backgroundColor = Placeholder.RaceColour(u.Core);
            token.Add(Placeholder.Label((u.Team == 1 && u.IsHero ? "Hollow " : "") + u.Name, "token__name"));
            var hp = new VisualElement(); hp.AddToClassList("token__hp");
            var fill = new VisualElement(); fill.AddToClassList("token__hpfill");
            hp.Add(fill); token.Add(hp);
            _field.Add(token);
            return (token, fill);
        }
    }
}
