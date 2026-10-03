// Local stand-in for the subset of NUnit that Unity's Test Runner provides.
// Only compiled by tools/csharp/test.sh; Unity uses the real NUnit.
// If a test needs an NUnit feature that is missing here, add it here (keep the real NUnit signature).
using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework
{
    [AttributeUsage(AttributeTargets.Class)] public sealed class TestFixtureAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Method)] public sealed class TestAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Method)] public sealed class SetUpAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class TestCaseAttribute : Attribute
    {
        public object[] Arguments { get; }
        public TestCaseAttribute(params object[] args) { Arguments = args ?? new object[] { null }; }
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class TestCaseSourceAttribute : Attribute
    {
        public string SourceName { get; }
        public TestCaseSourceAttribute(string sourceName) { SourceName = sourceName; }
    }

    public sealed class AssertionException : Exception { public AssertionException(string m) : base(m) { } }

    public static class Assert
    {
        static bool IsNumber(object o) => o is sbyte || o is byte || o is short || o is ushort || o is int || o is uint || o is long || o is ulong || o is float || o is double || o is decimal;
        static string Msg(string m) => string.IsNullOrEmpty(m) ? "" : " — " + m;
        public static void Fail(string message = null) => throw new AssertionException("Failed" + Msg(message));
        public static void AreEqual(object expected, object actual, string message = null)
        {
            bool eq = IsNumber(expected) && IsNumber(actual) ? Convert.ToDouble(expected) == Convert.ToDouble(actual) : Equals(expected, actual);
            if (!eq) throw new AssertionException($"Expected {Show(expected)} but was {Show(actual)}{Msg(message)}");
        }
        public static void AreEqual(double expected, double actual, double delta, string message = null)
        {
            if (double.IsNaN(actual) || Math.Abs(expected - actual) > delta) throw new AssertionException($"Expected {expected} ±{delta} but was {actual}{Msg(message)}");
        }
        public static void AreNotEqual(object notExpected, object actual, string message = null)
        {
            if (Equals(notExpected, actual)) throw new AssertionException($"Expected not {Show(notExpected)}{Msg(message)}");
        }
        public static void IsTrue(bool c, string message = null) { if (!c) throw new AssertionException("Expected true" + Msg(message)); }
        public static void IsFalse(bool c, string message = null) { if (c) throw new AssertionException("Expected false" + Msg(message)); }
        public static void IsNull(object o, string message = null) { if (o != null) throw new AssertionException($"Expected null but was {Show(o)}{Msg(message)}"); }
        public static void IsNotNull(object o, string message = null) { if (o == null) throw new AssertionException("Expected not null" + Msg(message)); }
        public static void Greater(double a, double b, string message = null) { if (!(a > b)) throw new AssertionException($"Expected {a} > {b}{Msg(message)}"); }
        public static void GreaterOrEqual(double a, double b, string message = null) { if (!(a >= b)) throw new AssertionException($"Expected {a} >= {b}{Msg(message)}"); }
        public static void Less(double a, double b, string message = null) { if (!(a < b)) throw new AssertionException($"Expected {a} < {b}{Msg(message)}"); }
        public static void LessOrEqual(double a, double b, string message = null) { if (!(a <= b)) throw new AssertionException($"Expected {a} <= {b}{Msg(message)}"); }
        public static void DoesNotThrow(TestDelegate code, string message = null)
        {
            try { code(); }
            catch (Exception e) { throw new AssertionException($"Expected no exception but got {e.GetType().Name}: {e.Message}{Msg(message)}"); }
        }

        public static T Throws<T>(TestDelegate code, string message = null) where T : Exception
        {
            try { code(); }
            catch (T e) { return e; }
            catch (Exception e) { throw new AssertionException($"Expected {typeof(T).Name} but got {e.GetType().Name}: {e.Message}{Msg(message)}"); }
            throw new AssertionException($"Expected {typeof(T).Name} but nothing was thrown{Msg(message)}");
        }
        static string Show(object o) => o == null ? "null" : o is string s ? "\"" + s + "\"" : o.ToString();
    }
    public delegate void TestDelegate();

    public static class CollectionAssert
    {
        public static void AreEquivalent(IEnumerable expected, IEnumerable actual, string message = null)
        {
            var e = new List<object>(); foreach (var o in expected) e.Add(o);
            var a = new List<object>(); foreach (var o in actual) a.Add(o);
            bool eq = e.Count == a.Count;
            foreach (var o in e) { if (!eq) break; int i = a.FindIndex(x => Equals(x, o)); if (i < 0) eq = false; else a.RemoveAt(i); }
            if (!eq) throw new AssertionException($"Expected the same items as [{string.Join(", ", e)}] in any order" + (string.IsNullOrEmpty(message) ? "" : " — " + message));
        }

        public static void AreEqual(IEnumerable expected, IEnumerable actual, string message = null)
        {
            var e = new List<object>(); foreach (var o in expected) e.Add(o);
            var a = new List<object>(); foreach (var o in actual) a.Add(o);
            bool eq = e.Count == a.Count;
            for (int i = 0; eq && i < e.Count; i++) eq = Equals(e[i], a[i]);
            if (!eq) throw new AssertionException($"Expected [{string.Join(", ", e)}] but was [{string.Join(", ", a)}]" + (string.IsNullOrEmpty(message) ? "" : " — " + message));
        }

        public static void Contains(IEnumerable collection, object item, string message = null)
        {
            foreach (var x in collection) if (Equals(x, item)) return;
            throw new AssertionException($"Collection does not contain {item}" + (message == null ? "" : " — " + message));
        }
        public static void IsEmpty(IEnumerable collection, string message = null)
        {
            var items = new List<string>();
            foreach (var x in collection) items.Add(x?.ToString());
            if (items.Count > 0) throw new AssertionException($"Expected empty but had {items.Count}: {string.Join("; ", items.GetRange(0, Math.Min(8, items.Count)))}" + (message == null ? "" : " — " + message));
        }
    }
}
