using System;
using System.Text;
using System.Collections.Generic;
using AngleSharp;

/// <summary>
/// Settings for <see cref="Wordify"/> post processor.
/// </summary>
public struct TextifySettings
{
    /// <summary>
    /// If set to true, page title will be included in result.
    /// </summary>
    public bool IncludeTitle { get; set; }

    /// <summary>
    /// If set to true, extracted text is sanitized.
    /// </summary>
    public bool SanitizeText { get; set; }

    /// <summary>
    /// Maximum allowed length of page to scrape. If null, will be to to a default
    /// of 100000.
    /// </summary>
    /// <remarks>
    /// An high limit with numerous pages to scrape can be resource intensive.
    /// </remarks>
    public uint? MaximumLength { get; set; }
}

/// <summary>
/// Data produced by <see cref="Textify"/> post processor.
/// </summary>
public struct TextifyData
{
    public TextifyData(string text)
    {
        Text = text;
    }

    public string Text
    {
        get;
        private set;
    }

    public override string ToString()
    {
        return Text;
    }
}

public class Textify : PostProcessor
{
    private readonly TextifySettings _settings;

    public Textify(object settings) : base(settings)
    {
        if (!(settings is TextifySettings)) {
            throw new NotSupportedException(
                $"{nameof(settings)} must be of {nameof(TextifySettings)} type");
        }
        _settings = (TextifySettings)Settings;
    }

    public override IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
    {
        var limit = _settings.MaximumLength ?? 100000;
        var builder = new StringBuilder(512);
        foreach (var result in results) {
            using (var document = Context.ActiveContext.OpenAsync(result.Url)
                .GetAwaiter().GetResult())
            {
                if (document.ToHtml().Length <= limit) {
                    yield return result.Clone(new TextifyData(
                        BuildText(document.TextSelectorAll(
                            _settings.IncludeTitle, _settings.SanitizeText))));
                }
                else {
                    yield return result;
                }
            }
        }

        string BuildText(IEnumerable<string> text)
        {
            builder.Length = 0;
            foreach (var element in text) {
                builder.Append(text);
                builder.Append(' ');
            }
            return builder.ToString(0, builder.Length - 1);
        }
    }
}