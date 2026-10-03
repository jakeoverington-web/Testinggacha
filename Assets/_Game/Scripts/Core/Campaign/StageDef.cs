using System.Collections.Generic;
using Gacha.Core.Data;

namespace Gacha.Core.Campaign
{
    /// <summary>One enemy in a stage: a roster hero (shown as a Hollow echo) at a level and star rank.</summary>
    public sealed class EnemyDef
    {
        public string Hero;
        public int Level = 1, Stars = 1;
    }

    /// <summary>One campaign stage (stages.json; decisions row 34). Ids ch01_s01 … ch20_s30 are stable forever.</summary>
    public sealed class StageDef
    {
        public string Id;
        public int Index, Chapter, Stage;
        public bool Gate, Boss;
        public List<string> Packages = new List<string>();
        public List<EnemyDef> Enemies = new List<EnemyDef>();
        public long Power;
        public Dictionary<string, double> FirstClear = new Dictionary<string, double>();

        public static List<StageDef> ListFromJson(string json)
        {
            var list = new List<StageDef>();
            foreach (var n in Node.Of(Json.Parse(json)).Nodes("stages")) list.Add(From(n));
            return list;
        }

        public static StageDef From(Node n)
        {
            var s = new StageDef
            {
                Id = n.Str("id"), Index = (int)n.Num("index"), Chapter = (int)n.Num("chapter"), Stage = (int)n.Num("stage"),
                Gate = n.Bool("gate"), Boss = n.Bool("boss"), Packages = n.Strs("packages"), Power = (long)n.Num("power")
            };
            foreach (var e in n.Nodes("enemies"))
                s.Enemies.Add(new EnemyDef { Hero = e.Str("hero"), Level = (int)e.Num("level", 1), Stars = (int)e.Num("stars", 1) });
            var fc = n.Obj("firstClear");
            if (fc != null) foreach (var k in fc.Keys) s.FirstClear[k] = fc.Num(k);
            return s;
        }

        public Dictionary<string, object> ToJson()
        {
            var enemies = new List<object>();
            foreach (var e in Enemies)
                enemies.Add(new Dictionary<string, object> { ["hero"] = e.Hero, ["level"] = e.Level, ["stars"] = e.Stars });
            var fc = new Dictionary<string, object>();
            foreach (var kv in FirstClear) fc[kv.Key] = kv.Value;
            return new Dictionary<string, object>
            {
                ["id"] = Id, ["index"] = Index, ["chapter"] = Chapter, ["stage"] = Stage, ["gate"] = Gate, ["boss"] = Boss,
                ["packages"] = new List<object>(Packages), ["enemies"] = enemies, ["power"] = Power, ["firstClear"] = fc
            };
        }

        public static string ListToJson(List<StageDef> stages, bool pretty = false, string schema = null)
        {
            var list = new List<object>();
            foreach (var s in stages) list.Add(s.ToJson());
            var root = new Dictionary<string, object>();
            if (schema != null) root["_schema"] = schema;
            root["stages"] = list;
            return Json.Write(root, pretty);
        }
    }
}
