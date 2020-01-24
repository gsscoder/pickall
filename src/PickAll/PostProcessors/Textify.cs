using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace PickAll
{
    /// <summary>Settings for <c>Textify</c> post processor.</summary>
    public struct TextifySettings
    {
        int _noiseLength;
        int? _maximumLength;

        /// <summary>If set to true, page title will be included in result.</summary>
        public bool IncludeTitle { get; set; }

        /// <summary>If set to true, extracted text is sanitized.</summary>
        public bool SanitizeText { get; set; }

        /// <summary>Maximum allowed length of the page to scrape. If null, will be to to a default
        /// of 100000.</summary>
        /// <remarks>An high limit with numerous pages to scrape can be resource intensive.</remarks>
        public int? MaximumLength
        {
            get { return _maximumLength; }
            set
            {
                if (value.HasValue) Guard.AgainstNegative("MaximumLength", value.Value);
                _maximumLength = value;
            }
        }

        /// <summary>Length of words to be considered noise.</summary>
        public int NoiseLength
        {
            get { return _noiseLength; }
            set
            {
                Guard.AgainstNegative("NoiseLength", value);
                _noiseLength = value;
            }
        }
    }

    /// <summary>Data produced by <c>Textify</c> post processor.</summary>
    public struct TextifyData
    {
        public TextifyData(string text) => Text = text;

        public string Text { get; private set; }

        public override string ToString() => Text;
    }

    /// <summary>Extracts all text from results URLs.</summary>
    public class Textify : PostProcessor
    {
        readonly TextifySettings _settings;
        

        public Textify(object settings) : base(settings)
        {
            if (!(settings is TextifySettings)) {
                throw new NotSupportedException(
                    $"{nameof(settings)} must be of {nameof(TextifySettings)} type");
            }
            _settings = (TextifySettings)Settings;
        }

        public override bool PublishEvents { get { return true; } }

        public override IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
        {
            var builder = new StringBuilder(512);
            var limit = _settings.MaximumLength ?? 100000;            
            foreach (var result in results) {
                var document = Context.Fetching.FetchAsync(result.Url).RunSynchronously<IFetchedDocument>();
                if (document.Equals(FetchedDocument.Empty)) continue;
                if (document.Length <= limit) continue;

                if (_settings.IncludeTitle) {
                    builder.Append(document.ElementSelector("title"));
                    builder.Append(' ');
                }
                builder.Append(AllTextContent(document)); 
                    yield return result.Clone(new TextifyData(builder.ToString().TrimEnd()));
            }

            string AllTextContent(IFetchedDocument document)
            {
                var content = new StringBuilder(512);
                content.Append(JoinAndRefine(document.ElementSelectorAll("div")));
                content.Append(JoinAndRefine(document.ElementSelectorAll("p")));
                content.Append(JoinAndRefine(document.ElementSelectorAll("h1")));
                content.Append(JoinAndRefine(document.ElementSelectorAll("h2")));
                content.Append(JoinAndRefine(document.ElementSelectorAll("h3")));
                content.Append(JoinAndRefine(document.ElementSelectorAll("h4")));
                content.Append(JoinAndRefine(document.ElementSelectorAll("h5")));
                content.Append(JoinAndRefine(document.ElementSelectorAll("h6")));
                return content.ToString();
                string JoinAndRefine(IEnumerable<string> texts) {
                    return string.Concat(
                        string.Join(" ",
                            from text in texts
                            select text.StripMl().NormalizeWhiteSpace().Sanitize()),
                            " ");
                }
            }
        }
    }
}