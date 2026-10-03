using System.Collections.Generic;
using Gacha.Game;
using Gacha.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gacha.Editor
{
    /// <summary>
    /// Builds Scenes/Main.unity and UI/PanelSettings.asset from code, so no scene or asset YAML is hand-edited (hard rule 5).
    /// Run from the menu Gacha/Build Main Scene (or the Unity CLI). Safe to re-run: it rebuilds both.
    /// </summary>
    public static class SceneBuilder
    {
        public const string ScenePath = "Assets/_Game/Scenes/Main.unity";
        public const string PanelPath = "Assets/_Game/UI/PanelSettings.asset";
        public const string ThemePath = "Assets/_Game/UI/GameTheme.tss";

        [MenuItem("Gacha/Build Main Scene")]
        public static void Build()
        {
            var panel = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelPath);
            if (panel == null)
            {
                panel = ScriptableObject.CreateInstance<PanelSettings>();
                AssetDatabase.CreateAsset(panel, PanelPath);
            }
            panel.themeStyleSheet = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(ThemePath);
            panel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            panel.referenceResolution = new Vector2Int(1080, 2340);   // decisions: reference resolution, portrait
            panel.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            panel.match = 1f;
            EditorUtility.SetDirty(panel);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cam = new GameObject("Main Camera", typeof(Camera));
            cam.tag = "MainCamera";
            var c = cam.GetComponent<Camera>();
            c.clearFlags = CameraClearFlags.SolidColor;
            c.backgroundColor = new Color(0.04f, 0.04f, 0.045f);   // Night Gold black (row 24)

            var root = new GameObject("Game", typeof(GameService), typeof(UIDocument), typeof(UiRoot));
            root.GetComponent<UIDocument>().panelSettings = panel;
            var ui = root.GetComponent<UiRoot>();
            ui.Common = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_Game/UI/Common/common.uss");
            ui.Map = Tree("CampaignMap/campaign_map");
            ui.PreBattle = Tree("PreBattle/pre_battle");
            ui.Battle = Tree("Battle/battle");
            ui.Result = Tree("Result/result");
            ui.Heroes = Tree("Heroes/heroes");
            ui.HeroDetail = Tree("HeroDetail/hero_detail");
            ui.Dev = Tree("Dev/dev");

            if (!AssetDatabase.IsValidFolder("Assets/_Game/Scenes")) AssetDatabase.CreateFolder("Assets/_Game", "Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            var scenes = new List<EditorBuildSettingsScene> { new EditorBuildSettingsScene(ScenePath, true) };
            EditorBuildSettings.scenes = scenes.ToArray();
            AssetDatabase.SaveAssets();
            Debug.Log("Gacha: built " + ScenePath);
        }

        static VisualTreeAsset Tree(string path)
        {
            var t = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Game/UI/" + path + ".uxml");
            if (t == null) throw new System.IO.FileNotFoundException("Missing UXML " + path);
            return t;
        }
    }
}
