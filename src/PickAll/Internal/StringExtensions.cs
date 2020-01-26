using System;
using System.Text;
using System.Text.RegularExpressions;

static class StringExtensions
{
    static Regex _stripMl = new Regex(@"<[^>]*>", RegexOptions.Compiled | RegexOptions.Multiline);

    public static bool IsAlphanumeric(this string value)
    {
        foreach (var @char in value.ToCharArray()) {
            if (!char.IsLetterOrDigit(@char) || char.IsWhiteSpace(@char)) {
                return false;
            }
        }
        return true;
    }

    public static string Sanitize(this string value, bool normalizeWhiteSpace = true)
    {
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
    
    public static string NormalizeWhiteSpace(this string value)
    {
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

    public static string StripMl(this string value)
    {
        return _stripMl.Replace(value, string.Empty);
    }

    public static string StripByLength(this string value, int length)
    {
        if (length < 0) throw new ArgumentException(nameof(length));
        if (length == 0) return value;

        var stripByLen = new Regex(
            string.Concat(@"\b\w{1,", length, @"}\b"),
            RegexOptions.Compiled | RegexOptions.Multiline);
        return stripByLen.Replace(value, string.Empty);
    }
}