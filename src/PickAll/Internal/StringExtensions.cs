// From: https://github.com/gsscoder/CSharpx/blob/master/src/CSharpx/StringExtensions.cs

using System;
using System.Text;
using System.Text.RegularExpressions;

static class StringExtensions
{
    static Regex _stripMl = new Regex(@"<[^>]*>", RegexOptions.Compiled | RegexOptions.Multiline);

    /// <summary>Determines if a string is composed only by alphanumeric characters.</summary>
    public static bool IsAlphanumeric(this string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        foreach (var @char in value.ToCharArray()) {
            if (!char.IsLetterOrDigit(@char) || char.IsWhiteSpace(@char)) {
                return false;
            }
        }
        return true;
    }

    /// <summary>Sanitizes a string removing non alphanumeric characters and optionally normalizing
    /// white spaces.</summary>
    public static string Sanitize(this string value, bool normalizeWhiteSpace = true)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        var builder = new StringBuilder(value.Length);
        foreach (var @char in value) {
            if (char.IsLetterOrDigit(@char)) {
                builder.Append(@char);
            }
            else if (char.IsWhiteSpace(@char)) {
                if (normalizeWhiteSpace) {
                    builder.Append(' ');
                } else {
                    builder.Append(@char);
                }
            }
        }
        return builder.ToString();
    }

    /// <summary>Normalizes any white space character to a single white space.</summary>
    public static string NormalizeWhiteSpace(this string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        var trimmed = value.Trim();
        var builder = new StringBuilder(trimmed.Length);
        var lastIndex = trimmed.Length - 2;
        for (var i = 0; i < trimmed.Length; i++) {
            var @char = trimmed[i];
            if (char.IsWhiteSpace(@char)) {
                if (i != lastIndex && !char.IsWhiteSpace(trimmed[i + 1])) {
                    builder.Append(' ');
                }
            }
            else {
                builder.Append(@char);
            }
        }
        return builder.ToString();
    }

    /// <summary>Removes markup from a string.</summary>
    public static string StripMl(this string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        return _stripMl.Replace(value, string.Empty);
    }

    /// <summary>Removes words of a given length.</summary>
    public static string StripByLength(this string value, int length)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (length < 0) throw new ArgumentException(nameof(length));
        if (length == 0) return value;

        var stripByLen = new Regex(
            string.Concat(@"\b\w{1,", length, @"}\b"),
            RegexOptions.Compiled | RegexOptions.Multiline);
        return stripByLen.Replace(value, string.Empty);
    }
}