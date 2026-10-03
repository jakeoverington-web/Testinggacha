using Gacha.Core.Campaign;
using UnityEngine.UIElements;

namespace Gacha.UI
{
    /// <summary>
    /// Campaign map (Game Modes and Progression Plan, Campaign: The map): one chapter's 30 banners winding up the screen,
    /// the idle chest, auto mode and unlock badges (row 35: a badge on the map, never a pop-up).
    /// Arg: a chapter number to show, or null for the chapter of the next stage.
    /// </summary>
    public sealed class CampaignMapController : IScreen
    {
        /// <summary>Unlocks (decisions row 35): name and the chapter that opens it. The modes themselves arrive in phase 4.</summary>
        static readonly (string name, int chapter)[] Unlocks =
            { ("Endless Stair", 2), ("Star ranks", 3), ("Rivals", 4), ("Boss mode", 5), ("Race towers", 6) };

        ScreenRouter _r;
        VisualElement _root;
        int _chapter;
        float _refresh;

        int NextStage => System.Math.Min(_r.State.HighestCleared + 1, _r.Data.Stages.Count);
        int LastChapter => _r.Data.Stages.Count / Expected.StagesPerChapter;

        public void Bind(VisualElement root, ScreenRouter router, object arg)
        {
            _r = router; _root = root;
            _chapter = arg is int c ? c : Expected.Chapter(NextStage);
            root.Q<Button>("prev").clicked += () => { if (_chapter > 1) { _chapter--; Fill(); } };
            root.Q<Button>("next").clicked += () => { if (_chapter < System.Math.Min(LastChapter, Expected.Chapter(NextStage))) { _chapter++; Fill(); } };
            root.Q<Button>("collect").clicked += () => { _r.State.Collect(_r.Data, _r.Game.Now); _r.Game.Save(); FillChest(); };
            var auto = root.Q<Toggle>("auto");
            auto.SetValueWithoutNotify(_r.State.Auto);
            auto.RegisterValueChangedCallback(e => { _r.State.Auto = e.newValue; _r.Game.Save(); });
            Fill();
        }

        public void Tick(float dt)
        {
            _refresh -= dt;
            if (_refresh > 0) return;
            _refresh = 1f;
            FillChest();
        }

        void Fill()
        {
            _root.Q<Label>("chapter-title").text = $"Chapter {_chapter}";
            FillBadges();
            FillBanners();
            FillChest();
        }

        void FillBadges()
        {
            var box = _root.Q("badges");
            box.Clear();
            foreach (var (name, ch) in Unlocks)
            {
                bool open = NextStage > (ch - 1) * Expected.StagesPerChapter;
                var b = Placeholder.Label(open ? name + " ✦" : $"{name} · ch {ch}", "badge");
                if (open) b.AddToClassList("badge--open");
                box.Add(b);
            }
        }

        void FillBanners()
        {
            var scroll = _root.Q<ScrollView>("banners");
            scroll.Clear();
            Button current = null;
            int first = (_chapter - 1) * Expected.StagesPerChapter + 1;
            // Stage 30 at the top, stage 1 at the bottom: the path winds up the screen.
            for (int i = first + Expected.StagesPerChapter - 1; i >= first; i--)
            {
                if (i > _r.Data.Stages.Count) continue;
                var s = _r.Data.Stages[i - 1];
                int index = i;
                var b = new Button(() => { if (index <= NextStage) _r.Show("prebattle", index); })
                {
                    text = $"{s.Chapter}-{s.Stage}{(s.Boss ? "  Boss" : s.Gate ? "  Gate" : "")}\nPower {Placeholder.Short(s.Power)}"
                };
                b.AddToClassList("btn"); b.AddToClassList("banner");
                b.AddToClassList((s.Stage % 4) switch { 0 => "banner--mid", 1 => "banner--left", 2 => "banner--mid", _ => "banner--right" });
                if (s.Gate) b.AddToClassList("banner--gate");
                if (s.Boss) b.AddToClassList("banner--boss");
                if (i <= _r.State.HighestCleared) b.AddToClassList("banner--cleared");
                else if (i == NextStage) { b.AddToClassList("banner--current"); b.text = "▲ " + b.text; current = b; }
                else { b.AddToClassList("banner--locked"); b.SetEnabled(false); }
                scroll.Add(b);
            }
            if (current != null)
            {
                // Scroll once the banners have a size; scrolling before layout does nothing.
                EventCallback<GeometryChangedEvent> onLayout = null;
                onLayout = _ => { current.UnregisterCallback(onLayout); scroll.ScrollTo(current); };
                current.RegisterCallback(onLayout);
            }
        }

        void FillChest()
        {
            var st = _r.State; var idle = _r.Data.Idle; int h = st.HighestCleared;
            st.Chest.Settle(_r.Game.Now, h, idle);
            _root.Q<Label>("wallet").text =
                $"Gold {Placeholder.Short(st.Wallet.Get("gold"))}   Hero XP {Placeholder.Short(st.Wallet.Get("heroXp"))}   Starlight {Placeholder.Short(st.Wallet.Get("starlight"))}";
            _root.Q<Label>("chest-rates").text = h == 0
                ? "Clear stage 1-1 to start earning."
                : $"Per hour: {Placeholder.Short(idle.PerHour("gold", h))} gold · {Placeholder.Short(idle.PerHour("heroXp", h))} Hero XP · {idle.PerHour("starlight", h):0.#} Starlight";
            double hours = st.Chest.AccruedSeconds / 3600;
            _root.Q<Label>("chest-held").text =
                $"In the chest ({hours:0.0} of {idle.CapHours:0} h): {Placeholder.Short(st.Chest.Get("gold"))} gold · {Placeholder.Short(st.Chest.Get("heroXp"))} Hero XP · {st.Chest.Get("starlight"):0} Starlight";
        }
    }
}
