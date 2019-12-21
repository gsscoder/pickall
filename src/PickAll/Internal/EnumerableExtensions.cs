using System;
using System.Collections.Generic;

namespace PickAll.Internal
{
    static class EnumerableExtensions
    {
        public static IEnumerable<object> Concat<T>(this IEnumerable<object> collection, T newElement)
        {
            foreach (var element in collection) {
                yield return element;
            }
            yield return newElement;
        }

        public static IEnumerable<object> Exclude(this IEnumerable<object> collection, Type type)
        {
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

        public static IEnumerable<object> Exclude<T>(this IEnumerable<object> collection)
        {
            return collection.Exclude(typeof(T));
        }

        /// <summary>
        /// Based on MoreLINQ one (github.com/morelinq).
        /// </summary>
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