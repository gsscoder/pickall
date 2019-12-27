//Use project level define(s) when referencing with Paket.
//#define CSX_STRING_EXT_INTERNAL // Uncomment this to set StringExtensions accessibility to internal.
//#define CSX_ARRAY_EXT_INTERNAL // Uncomment this to set ArrayExtensions accessibility to internal.

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
        /// <summary>
        /// Checks if a string is composed only by alphanumeric characters.
        /// </summary>
        /// <param name="@string">The string to check.</param>
        /// <returns>True if it's alphanumeric, otherwise false.</returns>
        public static bool IsAlphanumeric(this string @string)
        {
            foreach (var @char in @string.ToCharArray()) {
                if (!char.IsLetterOrDigit(@char) || char.IsWhiteSpace(@char)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if a string is contains any kind of whitespaces.
        /// </summary>
        /// <param name="@string">The string to check.</param>
        /// <returns>True if contains whitspaces, otherwise false</returns>
        public static bool ContainsWhiteSpace(this string @string)
        {
            foreach (var @char in @string.ToCharArray()) {
                if (char.IsWhiteSpace(@char)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Repeats a string for a given number of times using a seperator.
        /// </summary>
        /// <param name="@string">THe string to repeat</param>
        /// <param name="count">Number of times to repeat the string.</param>
        /// <param name="separator">The separator to use in the returned string.</param>
        /// <returns>A string repeated a given number of times.</returns>
        public static string Repeat(this string @string, int count, string separator = " ")
        {
            if (count < 0) {
                throw new ArgumentException(nameof(count));
            }

            return string.Join(separator, Enumerable.Repeat(@string, count).ToArray());
        }

        /// <summary>
        /// Applies a given function to nth-word of string.
        /// </summary>
        /// <param name="@string">The string to which apply the function.</param>
        /// <param name="index">The word index.</param>
        /// <param name="modifier">The function to apply to the word.</param>
        /// <returns></returns>
        public static string ApplyToWord(this string @string, int index, Func<string, string> modifier)
        {
            if (index < 0) {
                throw new ArgumentException(nameof(index));
            }

            var words = @string.Split().ToArray();
            words[index] = modifier(words[index]);
            return string.Join(" ", words);
        }

        /// <summary>
        /// Gets a random index of a word that optionally satisfies a function.
        /// </summary>
        /// <param name="@string">The string of interest.</param>
        /// <param name="validator">The function to validate the random word.</param>
        /// <returns></returns>
        public static int RandomWordIndex(this string @string, Func<string, bool> validator = null)
        {
            Func<string, bool> _nullValidator = _ => true;
            var _validator = validator ?? _nullValidator;

            var words = @string.Split();
            var index = new Random().Next(0,  words.Length - 1);
            if (_validator(words[index])) {
                return index;
            }
            return RandomWordIndex(@string, validator);
        }

        /// <summary>
        /// Mangles a word with a non alphanumeric character as prefix or suffix.
        /// </summary>
        /// <param name="word">The word to mangle (must not containg spaces).</param>
        /// <returns>The mangled word.</returns>
        public static string Mangle(this string word)
        {
            if (word.ContainsWhiteSpace()) {
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

        /// <summary>
        /// Reduces a collection of strings to a collection of words.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>A collection of words.</returns>
        public static IEnumerable<string> ToWords(this IEnumerable<string> collection)
        {
            foreach (var element in collection) {
                var words = element.Split();
                foreach (var word in words) {
                    yield return word;
                }
            }
        }

        /// <summary>
        /// Inserts strings of an array between words of a string.
        /// </summary>
        /// <param name="@string">The string to intersect.</param>
        /// <param name="texts">The texts to intersect</param>
        /// <returns>A string with given texts between words.</returns>
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
        /// <summary>
        /// Sorts an array pure way.
        /// </summary>
        /// <param name="array">The source array.</param>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <returns>The sorted array.</returns>
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
        /// <param name="array">The array of interest.</param>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <returns>The item randomly chosen.</returns>
        public static T Choice<T>(this T[] array)
        {
            var index = new Random().Next(array.Length - 1);
            return array[index];
        }
    }
}