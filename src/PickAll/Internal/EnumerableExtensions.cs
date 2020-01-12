using System;
using System.Collections.Generic;
using System.Linq;

namespace PickAll.Internal
{
    static class EnumerableExtensions
    {
        public static IEnumerable<T> Add<T>(this IEnumerable<T> collection, T newElement)
        {
            foreach (var element in collection) {
                yield return element;
            }
            yield return newElement;
        }

        public static IEnumerable<T> Remove<T>(this IEnumerable<T> collection, Type type)
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

        public static IEnumerable<T> CastOnlySubclassOf<T>(this IEnumerable<object> collection)
        {
            foreach (var element in collection) {
                if (element.GetType().IsSubclassOf(typeof(T))) {
                    yield return (T)element;
                }
            }
        }

        // From CSharpx (github.com/gsscoder/csharpx)
        public static IEnumerable<T> Memoize<T>(this IEnumerable<T> source)
        {
            return source.GetType().IsArray ? source : source.ToArray();
        }

        // Based on MoreLINQ one (github.com/morelinq/MoreLINQ).
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