//Use project level define(s) when referencing with Paket.
#define CSX_STRING_EXT_INTERNAL // Uncomment this to set StringExtensions accessibility to internal.
#define CSX_ARRAY_EXT_INTERNAL // Uncomment this to set ArrayExtensions accessibility to internal.

using System;
using System.Collections.Generic;
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

        public static string Repeat(this string @string, int count, string separator = " ")
        {
            if (count < 0) {
                throw new ArgumentException(nameof(count));
            }

            return string.Join(separator, Enumerable.Repeat(@string, count).ToArray());
        }

        public static string ApplyToWord(this string @string, int index, Func<string, string> modifier)
        {
            if (index < 0) {
                throw new ArgumentException(nameof(index));
            }

            var words = @string.Split().ToArray();
            words[index] = modifier(words[index]);
            return string.Join(" ", words);
        }

        public static int WordIndex(this string @string, Func<string, bool> validator = null)
        {
            Func<string, bool> _nullValidator = _ => true;
            var _validator = validator ?? _nullValidator;

            var words = @string.Split();
            var index = new Random().Next(0,  words.Length - 1);
            if (_validator(words[index])) {
                return index;
            }
            return WordIndex(@string, validator);
        }

        public static string Mangle(this string word)
        {
            if (word.Contains(' ')) {
                throw new ArgumentException(nameof(word));
            }

            var nonAlphanumeric =
                new string[] {"!", "\"", "£", "$", "%", "&", "/", "(", ")", "="}.Choice();
            var prefix = new Random().Next(0, 1);
            if (prefix == 1) {
                return $"{word}{nonAlphanumeric}";
            }
            return $"{nonAlphanumeric}{word}";
        }

        public static IEnumerable<string> ToWords(this IEnumerable<string> collection)
        {
            foreach (var element in collection) {
                var words = element.Split();
                foreach (var word in words) {
                    yield return word;
                }
            }
        }

        public static string BetweenWords(this string @string, params string[] texts)
        {
            if (texts.Length == 0) {
                return @string;
            }

            var words = @string.Split().ToArray();
            return string.Join(" ", Generate());

            IEnumerable<string> Generate()
            {
                var enumerator = texts.GetEnumerator();
                for (var i = 0; i < words.Length; i++) {
                    yield return words[i];
                    if (enumerator.MoveNext())
                    {
                        yield return (string)enumerator.Current;
                    }
                }
            }
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