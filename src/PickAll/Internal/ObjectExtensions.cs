using System;
using System.Collections.Generic;

static class ObjectExtensions
{
    public static IEnumerable<T> Add<T>(this IEnumerable<T> collection, T newElement)
    {
        foreach (var element in collection) {
            yield return element;
        }
        yield return newElement;
    }

    public static IEnumerable<object> Map<T>(this IEnumerable<object> collection, Func<T, T> func,
        Func<T, bool> predicate = null)
    {
        foreach (var element in collection) {
            if (element.GetType().EqualsOrSubtype<T>() &&
                (predicate == null ||
                (predicate != null && predicate((T)element)))) {
                yield return func((T)element);
            }
            else {
                yield return element;
            }
        }
    }

    public static IEnumerable<object> Remove(this IEnumerable<object> collection, Type type)
    {
        bool removed = false;
        foreach (var element in collection) {
            if (!element.GetType().Equals(type)) {
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

    public static IEnumerable<object> Remove<T>(
        this IEnumerable<object> collection) => collection.Remove(typeof(T));

    public static IEnumerable<object> RemoveAll<T>(this IEnumerable<object> collection)
    {
        foreach (var element in collection) {
            if (!element.GetType().IsSubclassOf(typeof(T))) {
                yield return element;
            }
        }
    }
}
