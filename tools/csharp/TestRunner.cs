// Local test runner: finds [Test]/[TestCase]/[TestCaseSource] methods by reflection and runs them.
// Only compiled by tools/csharp/test.sh. Unity's Test Runner replaces this.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

public static class LocalTestRunner
{
    public static int Run(string filter)
    {
        var sw = Stopwatch.StartNew();
        int pass = 0, fail = 0;
        var failures = new List<string>();
        var asm = typeof(LocalTestRunner).Assembly;
        foreach (var type in asm.GetTypes().Where(t => t.GetCustomAttribute<TestFixtureAttribute>() != null).OrderBy(t => t.FullName))
        {
            foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => x.MetadataToken))
            {
                var cases = new List<object[]>();
                if (m.GetCustomAttribute<TestAttribute>() != null) cases.Add(new object[0]);
                foreach (var tc in m.GetCustomAttributes<TestCaseAttribute>()) cases.Add(tc.Arguments);
                foreach (var src in m.GetCustomAttributes<TestCaseSourceAttribute>())
                {
                    var f = (MemberInfo)type.GetField(src.SourceName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                            ?? (MemberInfo)type.GetProperty(src.SourceName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                            ?? type.GetMethod(src.SourceName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    object v = f is FieldInfo fi ? fi.GetValue(null) : f is PropertyInfo pi ? pi.GetValue(null) : ((MethodInfo)f).Invoke(null, null);
                    foreach (var item in (IEnumerable)v) cases.Add(item is object[] arr ? arr : new[] { item });
                }
                foreach (var args in cases)
                {
                    string name = $"{type.Name}.{m.Name}" + (args.Length > 0 ? "(" + string.Join(", ", args.Select(a => a?.ToString() ?? "null")) + ")" : "");
                    if (!string.IsNullOrEmpty(filter) && name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    try
                    {
                        var inst = Activator.CreateInstance(type);
                        foreach (var s in type.GetMethods().Where(x => x.GetCustomAttribute<SetUpAttribute>() != null)) s.Invoke(inst, null);
                        m.Invoke(inst, args);
                        pass++;
                    }
                    catch (TargetInvocationException e)
                    {
                        fail++;
                        var inner = e.InnerException;
                        var where = inner is AssertionException ? "" : "\n      " + string.Join("\n      ", (inner.StackTrace ?? "").Split('\n').Take(4));
                        failures.Add($"FAIL {name}\n      {inner.GetType().Name}: {inner.Message}{where}");
                    }
                }
            }
        }
        foreach (var f in failures) Console.WriteLine(f);
        Console.WriteLine($"{pass} passed, {fail} failed in {sw.Elapsed.TotalSeconds:0.00}s");
        return fail == 0 ? 0 : 1;
    }
}
