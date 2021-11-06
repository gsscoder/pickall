using System;

static class TypeExtensions
{
    public static bool EqualsOrSubtype<T>(this Type type) => type.Equals(typeof(T)) ||
                                                             type.IsSubclassOf(typeof(T));
}
