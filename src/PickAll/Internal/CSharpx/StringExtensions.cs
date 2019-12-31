//#define CSX_STRING_EXT_INTERNAL // Uncomment or define at build time to set StringExtensions accessibility to internal.

using System;
using System.Text;
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
        /// Determines if a string is composed only by letter characters.
        /// </summary>
        public static bool IsAlpha(this string value)
        {
            foreach (var @char in value.ToCharArray()) {
                if (!char.IsLetter(@char) || char.IsWhiteSpace(@char)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines if a string is composed only by alphanumeric characters.
        /// </summary>
        public static bool IsAlphanumeric(this string value)
        {
            foreach (var @char in value.ToCharArray()) {
                if (!char.IsLetterOrDigit(@char) || char.IsWhiteSpace(@char)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines if a string is contains any kind of white spaces.
        /// </summary>
        public static bool IsWhiteSpace(this string value)
        {
            foreach (var @char in value.ToCharArray()) {
                if (char.IsWhiteSpace(@char)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Replicates a string for a given number of times using a seperator.
        /// </summary>
        public static string Replicate(this string value, uint count, string separator = " ")
        {
            if (separator == null) throw new ArgumentNullException(nameof(separator));

            var builder = new StringBuilder();
            for (var i = 0; i < count; i++) {
                builder.Append(value);
                builder.Append(separator);
            }
            return builder.ToString(0, builder.Length - separator.Length);
        }

        /// <summary>
        /// Applies a given function to nth-word of string.
        /// </summary>
        public static string ApplyAt(this string value, int index, Func<string, string> modifier)
        {
            if (index < 0) throw new ArgumentException(nameof(index));

            var words = value.Split().ToArray();
            words[index] = modifier(words[index]);
            return string.Join(" ", words);
        }

        /// <summary>
        /// Selects a random index of a word that optionally satisfies a function.
        /// </summary>
        public static int ChoiceOfIndex(this string value, Func<string, bool> validator = null)
        {
            Func<string, bool> _nullValidator = _ => true;
            var _validator = validator ?? _nullValidator;

            var words = value.Split();
            var index = new Random().Next(0,  words.Length - 1);
            if (_validator(words[index])) {
                return index;
            }
            return ChoiceOfIndex(value, validator);
        }

        private static string[] _mangleChars =
            {"!", "\"", "£", "$", "%", "&", "/", "(", ")", "=", "?", "^", "[", "]", "*", "@", "°",
             "#", "§", ",", ";", ".", ":", "-", "_"};

        /// <summary>
        /// Mangles a string with a given number of non alphanumeric character in random positions.
        /// </summary>
        public static string Mangle(this string value, uint times = 1, uint maxLength = 1)
        {
            if (times > value.Length) throw new ArgumentException(nameof(times));
            if (times == 0 || maxLength == 0) return value;

            var random = new Random();
            var indexes = new List<int>((int)times);
            int uniqueNext()
                {
                    var index = random.Next(0, value.Length - 1);
                    if (indexes.Contains(index)) {
                        return uniqueNext();
                    }
                    return index;
                };
            for (var i = 0; i < times; i++) {
                indexes.Add(uniqueNext());
            }
            var mutations = indexes.OrderBy(index => index);

            var mangled = new StringBuilder(value.Length + (int)times * (int)maxLength);
            for (var i = 0; i < value.Length; i++) {
                mangled.Append(value[i]);
                if (mutations.Contains(i)) {
                    mangled.Append(
                        _mangleChars[random.Next(0, _mangleChars.Length - 1)]
                        .Replicate(maxLength, string.Empty));
                    
                }
            }
            return mangled.ToString();
        }

        /// <summary>
        /// Takes a value and a string and `intersperses' that value between its words.
        /// </summary>
        public static string Intersperse(this string value, params object[] values)
        {
            if (values.Length == 0) {
                return value;
            }
            return string.Join(" ", impl());
            IEnumerable<string> impl() {
                var words = value.Split();
                var count = words.Length;
                var last = count - 1;
                for (var i = 0; i < count; i++) {
                    yield return words.ElementAt(i);
                    if (i < values.Length) {
                        var element = values[i];
                        if (element.GetType() == typeof(string)) {
                            yield return (string)element;
                        } else {
                            yield return element.ToString();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sanitizes a string removing non alphanumeric characters and optionally normalizing
        /// white spaces.
        /// </summary>
        public static string Sanitize(this string value, bool normalizeWhiteSpace = true)
        {
            return impl().Aggregate<char, string>(string.Empty, (s, c) => $"{s}{c}");
            IEnumerable<char> impl() {
                foreach (var @char in value) {
                    if (Char.IsLetterOrDigit(@char)) {
                        yield return @char;
                    }
                    else if (Char.IsWhiteSpace(@char)) {
                        if (normalizeWhiteSpace) {
                            yield return ' ';
                        } else {
                            yield return @char;
                        }
                    }
                }
            }
        }
    }
}