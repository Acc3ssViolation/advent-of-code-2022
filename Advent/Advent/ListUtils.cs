using System;

namespace Advent
{
    internal static class ListUtils
    {
        public static string AggregateString<T>(this IEnumerable<T> list)
        {
            return list.Aggregate("", (a, b) => a.Length > 0 ? $"{a}, {b}" : (b?.ToString() ?? ""));
        }

        public static string AggregateString<T>(this IEnumerable<T> list, Func<T, string> func)
        {
            return list.Aggregate("", (a, b) => a.Length > 0 ? $"{a}, {func(b)}" : func(b));
        }
    }
}
