using System;
using System.Collections.Generic;
using System.Linq;

static class EnumerableExtensions
{
    public static IEnumerable<T> Memoize<T>(
        this IEnumerable<T> source) => source.GetType().IsArray ? source : source.ToArray();

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        var knownKeys = new HashSet<TKey>((IEqualityComparer<TKey>)null);
        foreach (var element in source) {
            if (knownKeys.Add(keySelector(element))) {
                yield return element;
            }
        }
    }
}