using System;
using System.Collections.Generic;
using System.Linq;

namespace PickAll
{
    static class EnumerableExtensions
    {
        // Based on MoreLINQ one (/github.com/morelinq)
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer = null)
        {
            var knownKeys = new HashSet<TKey>(comparer);
            foreach (var element in source) {
                if (knownKeys.Add(keySelector(element))) {
                    yield return element;
                }
            }
        }
    }
}