// From: https://github.com/gsscoder/CSharpx/blob/master/src/CSharpx/EnumerableExtensions.cs

using System;
using System.Collections.Generic;

static class EnumerableExtensions
{
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
