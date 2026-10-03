using UnityEngine;
using UnityEngine.UIElements;

namespace Gacha.UI
{
    /// <summary>Owns the screen router on the UIDocument; assets are assigned by the SceneBuilder editor script.</summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class UiRoot : MonoBehaviour
    {
        public VisualTreeAsset Map, PreBattle, Battle, Result, Heroes, HeroDetail, Dev;
        public StyleSheet Common;

        ScreenRouter _router;
        VisualElement _host;
        Rect _safe;

        /// <summary>Start, not OnEnable: every Awake (GameService loads data and the save there) has run by now.</summary>
        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            root.Clear();
            root.styleSheets.Add(Common);
            root.AddToClassList("app");
            _host = new VisualElement();
            _host.AddToClassList("app__host");
            root.Add(_host);
            // Bottom tab bar (phase 2): Campaign | Heroes; Modes arrives in phase 4.
            var tabs = new VisualElement();
            tabs.AddToClassList("tabbar");
            root.Add(tabs);
            ApplySafeArea();
            _router = new ScreenRouter(_host, this, tabs);
            _router.Show("map");
        }

        void Update()
        {
            if (Screen.safeArea != _safe) ApplySafeArea();
            _router?.Tick(Time.deltaTime);
        }

        /// <summary>Keep every screen inside the phone's safe area (notches, gesture bar).</summary>
        void ApplySafeArea()
        {
            var panel = _host?.panel;
            if (panel == null) return;   // retried next frame until the panel exists
            _safe = Screen.safeArea;
            var leftTop = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(_safe.xMin, Screen.height - _safe.yMax));
            var rightBottom = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(Screen.width - _safe.xMax, _safe.yMin));
            _host.style.paddingLeft = leftTop.x;
            _host.style.paddingTop = leftTop.y;
            _host.style.paddingRight = rightBottom.x;
            _host.style.paddingBottom = rightBottom.y;
        }
    }
}
