//#define CSX_ARRAY_EXT_INTERNAL // Uncomment or define at build time to set ArrayExtensions accessibility to internal.

using System;

namespace CSharpx
{
#if !CSX_ARRAY_EXT_INTERNAL
    public
#endif
    static class ArrayExtensions
    {
        /// <summary>
        /// Sorts an array pure way.
        /// </summary>
        public static T[] Sort<T>(this T[] array)
        {
            var copy = new T[array.Length];
            Array.Copy(array, copy, array.Length);
            Array.Sort(copy);
            return copy;
        }

        /// <summary>
        /// Chooses a random element from an array.
        /// </summary>
        public static T Choice<T>(this T[] array)
        {
            var index = new Random().Next(array.Length - 1);
            return array[index];
        }
    }
}