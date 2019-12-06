using System;
using System.Collections.Generic;
using System.Linq;

namespace PickAll
{
    static class EnumerableExtensions
    {
        public static IEnumerable<T> CloneWith<T>(this IEnumerable<T> collection, T newElement)
        {
            foreach (var element in collection) {
                yield return element;
            }
            yield return newElement;
        }

        public static IEnumerable<object> CloneWithout<T>(this IEnumerable<object> collection)
        {
            var type = typeof(T);
            bool removed = false;
            foreach (var element in collection) {
                if (element.GetType() != type) {
                    yield return element;
                }
                else {
                    if (!removed) {
                        removed = true;
                    }
                    else {
                        yield return element;
                    }
                }
            }
        }


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