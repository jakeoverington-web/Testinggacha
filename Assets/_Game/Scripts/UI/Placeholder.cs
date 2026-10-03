using Gacha.Core.Battle;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gacha.UI
{
    /// <summary>Flat-colour placeholders (hard rule 1): a tile per hero, coloured by race, with text labels.</summary>
    public static class Placeholder
    {
        /// <summary>Each core's light (decisions row 20), as flat colours.</summary>
        public static Color RaceColour(string core) => core switch
        {
            "high" => new Color32(0xC9, 0xA5, 0x4C, 0xFF),
            "dark" => new Color32(0x8E, 0x24, 0x33, 0xFF),
            "nature" => new Color32(0x2E, 0x7D, 0x4F, 0xFF),
            "ocean" => new Color32(0x1F, 0x5F, 0x8B, 0xFF),
            "arcane" => new Color32(0x4B, 0x3B, 0x8F, 0xFF),
            _ => new Color32(0x44, 0x44, 0x48, 0xFF),
        };

        public static VisualElement HeroTile(HeroDef h, string caption, bool hollow = false)
        {
            var tile = new VisualElement();
            tile.AddToClassList("tile");
            tile.style.backgroundColor = RaceColour(h.Core);
            if (hollow) tile.AddToClassList("tile--hollow");
            tile.Add(Label((hollow ? "Hollow " : "") + h.Name, "tile__name"));
            tile.Add(Label(h.Role, "tile__role"));
            if (!string.IsNullOrEmpty(caption)) tile.Add(Label(caption, "tile__caption"));
            return tile;
        }

        public static Label Label(string text, string cls)
        {
            var l = new Label(text);
            l.AddToClassList(cls);
            return l;
        }

        public static string Stars(int n) => n <= 5 ? new string('★', n) : new string('◆', n - 5) + " (" + n + "★)";

        /// <summary>12,345 → "12.3K"; 4,100,000 → "4.1M".</summary>
        public static string Short(double v) =>
            v >= 1e6 ? (v / 1e6).ToString("0.#") + "M" : v >= 1e4 ? (v / 1e3).ToString("0.#") + "K" : v.ToString("N0");
    }
}
