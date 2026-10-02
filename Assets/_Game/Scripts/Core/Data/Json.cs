using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Gacha.Core.Data
{
    /// <summary>
    /// Minimal engine-free JSON reader. Objects become Dictionary&lt;string, object&gt;, arrays List&lt;object&gt;,
    /// numbers double, plus string/bool/null. Enough for our data files; no reflection, no UnityEngine.
    /// </summary>
    public static class Json
    {
        public static object Parse(string text)
        {
            int i = 0;
            var v = Value(text, ref i);
            Skip(text, ref i);
            if (i != text.Length) throw Err(text, i, "trailing characters");
            return v;
        }

        static Exception Err(string s, int i, string what)
        {
            int line = 1; for (int k = 0; k < i && k < s.Length; k++) if (s[k] == '\n') line++;
            return new FormatException($"JSON: {what} at line {line}");
        }

        static void Skip(string s, ref int i) { while (i < s.Length && char.IsWhiteSpace(s[i])) i++; }

        static object Value(string s, ref int i)
        {
            Skip(s, ref i);
            if (i >= s.Length) throw Err(s, i, "unexpected end");
            char c = s[i];
            if (c == '{') return Obj(s, ref i);
            if (c == '[') return Arr(s, ref i);
            if (c == '"') return Str(s, ref i);
            if (c == 't' && Lit(s, ref i, "true")) return true;
            if (c == 'f' && Lit(s, ref i, "false")) return false;
            if (c == 'n' && Lit(s, ref i, "null")) return null;
            return Num(s, ref i);
        }

        static bool Lit(string s, ref int i, string lit)
        {
            if (string.CompareOrdinal(s, i, lit, 0, lit.Length) != 0) throw Err(s, i, "bad literal");
            i += lit.Length; return true;
        }

        static Dictionary<string, object> Obj(string s, ref int i)
        {
            var d = new Dictionary<string, object>();
            i++; Skip(s, ref i);
            if (s[i] == '}') { i++; return d; }
            while (true)
            {
                Skip(s, ref i);
                var k = Str(s, ref i);
                Skip(s, ref i);
                if (s[i] != ':') throw Err(s, i, "expected ':'");
                i++;
                d[k] = Value(s, ref i);
                Skip(s, ref i);
                if (s[i] == ',') { i++; continue; }
                if (s[i] == '}') { i++; return d; }
                throw Err(s, i, "expected ',' or '}'");
            }
        }

        static List<object> Arr(string s, ref int i)
        {
            var l = new List<object>();
            i++; Skip(s, ref i);
            if (s[i] == ']') { i++; return l; }
            while (true)
            {
                l.Add(Value(s, ref i));
                Skip(s, ref i);
                if (s[i] == ',') { i++; continue; }
                if (s[i] == ']') { i++; return l; }
                throw Err(s, i, "expected ',' or ']'");
            }
        }

        static string Str(string s, ref int i)
        {
            if (s[i] != '"') throw Err(s, i, "expected string");
            i++;
            var sb = new StringBuilder();
            while (i < s.Length)
            {
                char c = s[i++];
                if (c == '"') return sb.ToString();
                if (c != '\\') { sb.Append(c); continue; }
                char e = s[i++];
                switch (e)
                {
                    case 'n': sb.Append('\n'); break;
                    case 't': sb.Append('\t'); break;
                    case 'r': sb.Append('\r'); break;
                    case 'b': sb.Append('\b'); break;
                    case 'f': sb.Append('\f'); break;
                    case 'u': sb.Append((char)int.Parse(s.Substring(i, 4), NumberStyles.HexNumber)); i += 4; break;
                    default: sb.Append(e); break;
                }
            }
            throw Err(s, i, "unterminated string");
        }

        static double Num(string s, ref int i)
        {
            int start = i;
            while (i < s.Length && "+-0123456789.eE".IndexOf(s[i]) >= 0) i++;
            if (i == start) throw Err(s, i, "unexpected character");
            return double.Parse(s.Substring(start, i - start), NumberStyles.Float, CultureInfo.InvariantCulture);
        }
    }

    /// <summary>Read-only view over a parsed JSON object with typed getters and defaults.</summary>
    public sealed class Node
    {
        public readonly Dictionary<string, object> Raw;
        public Node(Dictionary<string, object> raw) { Raw = raw ?? new Dictionary<string, object>(); }
        public static Node Of(object o) => o is Dictionary<string, object> d ? new Node(d) : null;

        public bool Has(string k) => Raw.ContainsKey(k) && Raw[k] != null;
        public string Str(string k, string def = null) => Raw.TryGetValue(k, out var v) && v is string s ? s : def;
        public double Num(string k, double def = 0) => Raw.TryGetValue(k, out var v) && v is double d ? d : def;
        public bool Bool(string k, bool def = false) => Raw.TryGetValue(k, out var v) && v is bool b ? b : def;
        public Node Obj(string k) => Raw.TryGetValue(k, out var v) ? Of(v) : null;
        public List<object> List(string k) => Raw.TryGetValue(k, out var v) && v is List<object> l ? l : new List<object>();
        public List<Node> Nodes(string k) { var r = new List<Node>(); foreach (var o in List(k)) { var n = Of(o); if (n != null) r.Add(n); } return r; }
        public List<string> Strs(string k) { var r = new List<string>(); foreach (var o in List(k)) if (o is string s) r.Add(s); return r; }
        public IEnumerable<string> Keys => Raw.Keys;
    }
}
