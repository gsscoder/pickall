//Use project level define(s) when referencing with Paket.
#define CSX_STRING_EXT_INTERNAL // Uncomment this to set StringExtensions accessibility to internal.
#define CSX_ARRAY_EXT_INTERNAL // Uncomment this to set ArrayExtensions accessibility to internal.

using System;
using System.Linq;

namespace CSharpx
{
#if !CSX_STRING_EXT_INTERNAL
    public
#endif
    static class StringExtensions
    {
        public static bool IsAlphanumeric(this string @string)
        {
            foreach(var @char in @string.ToCharArray()) {
                if (!char.IsLetterOrDigit(@char) || char.IsWhiteSpace(@char)) {
                    return false;
                }
            }
            return true;
        }

        public static string Repeat(this string @string, int count)
        {
            if (count < 0) {
                throw new ArgumentException(nameof(count));
            }

            return string.Join(" ", Enumerable.Repeat(@string, count).ToArray());
        }
    }

#if !CSX_ARRAY_EXT_INTERNAL
    public
#endif
    static class ArrayExtensions
    {
        public static T[] Sort<T>(this T[] array)
        {
            var copy = new T[array.Length];
            Array.Copy(array, copy, array.Length);
            Array.Sort(copy);
            return copy;
        }

        public static T Choice<T>(this T[] array)
        {
            var index = new Random().Next(array.Length - 1);
            return array[index];
        }
    }
}