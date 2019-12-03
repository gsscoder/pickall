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
            foreach (var element in collection) {
                if (element.GetType() != typeof(T)) {
                    yield return element;
                }
            }
        }

        public static IEnumerable<T> CastOnlySubclassOf<T>(this IEnumerable<object> source)
        {
            return source.Where(item => item.GetType().IsSubclassOf(typeof(T))).Cast<T>();
        }

        public static IEnumerable<T> CastImplements<T>(this IEnumerable<object> source)
        {
            return source.Where(item => typeof(T).IsAssignableFrom(item.GetType())).Cast<T>();
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