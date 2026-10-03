using System.Collections.Generic;
using Gacha.Core.Battle;
using Gacha.Core.Campaign;
using UnityEngine.UIElements;

namespace Gacha.UI
{
    /// <summary>
    /// Live battle (redesign spec): steps the fight in whole ticks while you watch; HP and energy bars on every unit;
    /// your 5 portraits along the bottom. With manual ultimates a full-energy portrait grows into a tall card and a tap
    /// casts it (an input, logged for replay). AUTO toggles auto-casting; Skip finishes the fight instantly.
    /// Arg: the FightSession.
    /// </summary>
    public sealed class BattleViewController : IScreen
    {
        static readonly float[] Speeds = { 1f, 2f, 4f };

        ScreenRouter _r;
        VisualElement _root, _field;
        FightSession _session;
        Battle _b;
        int _speed;
        float _endDelay = 0.8f;
        /// <summary>Real time not yet played: the sim moves in whole ticks (0.1 s), and RunFor always runs at least one.</summary>
        double _pending;
        readonly Dictionary<int, (VisualElement el, VisualElement hp, VisualElement en)> _tokens = new Dictionary<int, (VisualElement, VisualElement, VisualElement)>();
        readonly Dictionary<int, (VisualElement el, VisualElement hp, VisualElement en)> _portraits = new Dictionary<int, (VisualElement, VisualElement, VisualElement)>();

        public void Bind(VisualElement root, ScreenRouter router, object arg)
        {
            _r = router; _root = root; _session = (FightSession)arg; _b = _session.Battle;
            var s = _r.Data.Stages[_session.StageIndex - 1];
            root.Q<Label>("title").text = $"Stage {s.Chapter}-{s.Stage}";
            _field = root.Q("field");
            var speed = root.Q<Button>("speed");
            speed.clicked += () => { _speed = (_speed + 1) % Speeds.Length; speed.text = $"Speed {Speeds[_speed]:0}×"; };
            root.Q<Button>("auto").clicked += () => _b.Queue(new BattleInput { Kind = InputKind.SetAuto, Team = 0, On = _b.ManualUltimates[0] });
            root.Q<Button>("skip").clicked += Finish;
            BuildPortraits();
            Draw();
        }

        public void Tick(float dt)
        {
            if (_b == null) return;
            if (!_b.Over)
            {
                _pending += dt * Speeds[_speed];
                while (_pending >= _b.T.Tick && !_b.Over) { _b.Step(); _pending -= _b.T.Tick; }
                Draw();
                return;
            }
            _endDelay -= dt;
            if (_endDelay <= 0) Finish();
        }

        void Finish()
        {
            if (_b == null) return;
            _b = null;
            _r.Finish(_session);
        }

        void BuildPortraits()
        {
            var row = _root.Q("portraits");
            foreach (var u in _b.Units)
            {
                if (u.Team != 0 || !u.IsHero) continue;
                int index = u.Index;
                var el = new VisualElement();
                el.AddToClassList("portrait");
                el.style.backgroundColor = Placeholder.RaceColour(u.Core);
                el.Add(Placeholder.Label(u.Name, "portrait__name"));
                el.Add(Placeholder.Label("ULTIMATE", "portrait__ult"));
                var (hp, en) = Bars(el);
                el.RegisterCallback<ClickEvent>(_ => TapPortrait(index));
                _portraits[index] = (el, hp, en);
                row.Add(el);
            }
        }

        void TapPortrait(int index)
        {
            if (_b == null || !_b.ManualUltimates[0]) return;
            var u = _b.Units[index];
            if (u.Alive && u.Energy >= 100 - 1e-9 && !u.UltRequested) _b.Queue(new BattleInput { Kind = InputKind.CastUltimate, Unit = index });
        }

        void Draw()
        {
            var t = _b.T;
            _root.Q<Label>("clock").text = $"{_b.Time:0}s / {t.TimeLimit:0}s";
            _root.Q<Button>("auto").text = _b.ManualUltimates[0] ? "AUTO: off" : "AUTO: on";
            foreach (var u in _b.Units)
            {
                if (!_tokens.TryGetValue(u.Index, out var tk)) tk = _tokens[u.Index] = MakeToken(u);
                tk.el.style.left = Length.Percent((float)((u.X + t.HalfWidth) / (2 * t.HalfWidth) * 100));
                tk.el.style.top = Length.Percent((float)((u.Y + t.HalfDepth) / (2 * t.HalfDepth) * 80 + 10));
                SetBars(tk.hp, tk.en, u);
                tk.el.EnableInClassList("token--dead", !u.Alive);
            }
            foreach (var kv in _portraits)
            {
                var u = _b.Units[kv.Key];
                SetBars(kv.Value.hp, kv.Value.en, u);
                kv.Value.el.EnableInClassList("portrait--dead", !u.Alive);
                kv.Value.el.EnableInClassList("portrait--ready", _b.ManualUltimates[0] && u.Alive && u.Energy >= 100 - 1e-9 && !u.UltRequested);
            }
        }

        static void SetBars(VisualElement hp, VisualElement en, Unit u)
        {
            hp.style.width = Length.Percent((float)(System.Math.Max(0, u.HpPct) * 100));
            en.style.width = Length.Percent((float)(System.Math.Clamp(u.Energy, 0, 100)));
        }

        static (VisualElement hp, VisualElement en) Bars(VisualElement parent)
        {
            var box = new VisualElement();
            var hpBar = new VisualElement(); hpBar.AddToClassList("bar-hp");
            var hp = new VisualElement(); hp.AddToClassList("bar-hp__fill"); hpBar.Add(hp);
            var enBar = new VisualElement(); enBar.AddToClassList("bar-en");
            var en = new VisualElement(); en.AddToClassList("bar-en__fill"); enBar.Add(en);
            box.Add(hpBar); box.Add(enBar);
            parent.Add(box);
            return (hp, en);
        }

        (VisualElement, VisualElement, VisualElement) MakeToken(Unit u)
        {
            var el = new VisualElement();
            el.AddToClassList("token");
            if (u.Team == 1) el.AddToClassList("token--enemy");
            if (u.IsSummon) el.AddToClassList("token--summon");
            el.style.backgroundColor = Placeholder.RaceColour(u.Core);
            el.Add(Placeholder.Label((u.Team == 1 && u.IsHero ? "Hollow " : "") + u.Name, "token__name"));
            var (hp, en) = Bars(el);
            _field.Add(el);
            return (el, hp, en);
        }
    }
}
