using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;

namespace PickAll
{
    /// <summary>
    /// Settings for <see cref="Wordify"/> post processor.
    /// </summary>
    public struct WordifySettings
    {
        int? _maximumLength;
        int _noiseLength;

        /// <summary>
        /// If set to true, page title will be included in result.
        /// </summary>
        public bool IncludeTitle { get; set; }

        /// <summary>
        /// Maximum allowed length of page to scrape. If null, will be to to a default
        /// of 100000.
        /// </summary>
        /// <remarks>
        /// An high limit with numerous pages to scrape can be resource intensive.
        /// </remarks>
        public int? MaximumLength
        {
            get { return _maximumLength; }
            set
            {
                if (value.HasValue) Guard.AgainstNegative("MaximumLength", value.Value);
                _maximumLength = value;
            }
        }

        /// <summary>
        /// Length of words to be considered noise.
        /// </summary>
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

    /// <summary>
    /// Data produced by <see cref="Wordify"/> post processor.
    /// </summary>
    public struct WordifyData
    {
        public WordifyData(IEnumerable<string> words)
        {
            Words = words;
        }

        public IEnumerable<string> Words
        {
            get;
            private set;
        }

        public override string ToString()
        {
            if (!Words.Any()) {
                return string.Empty;
            }
            var builder = new StringBuilder();
            foreach (var word in Words) {
                builder.Append(word);
                builder.Append(' ');
            }
            return builder.ToString(0, builder.Length - 1);
        }
    }

    /// <summary>
    /// Reduces documents identified by results URLs to a collection of words.
    /// </summary>
    public class Wordify : PostProcessor
    {
        private readonly WordifySettings _settings;

        public Wordify(object settings) : base(settings)
        {
            if (!(settings is WordifySettings)) {
                throw new NotSupportedException(
                    $"{nameof(settings)} must be of {nameof(WordifySettings)} type");
            }
            _settings = (WordifySettings)Settings;
        }

        public override bool PublishEvents { get { return true; } }

        public override IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
        {
            var limit = _settings.MaximumLength ?? 100000;
            foreach (var result in results) {
                using (var document = Context.ActiveContext.OpenAsync(result.Url)
                    .GetAwaiter().GetResult())
                {
                    if (document.ToHtml().Length <= limit) {
                        yield return result.Clone(new WordifyData(ExtractWords(document)));
                    }
                    else {
                        yield return result;
                    }
                }
            }
        }

    #if DEBUG
        public
    #endif
        IEnumerable<string> ExtractWords(IDocument document)
        {
            Func<string, bool> couldBeNoise = _settings.NoiseLength == 0
                ? couldBeNoise =  _ => false
                : w => w.Length <= _settings.NoiseLength;
            var texts = document.TextSelectorAll(_settings.IncludeTitle, sanitizeText: true);
            foreach (var text in texts) {
                var words = text.Split();
                foreach (var word in from @this in words
                                        where !couldBeNoise(@this)
                                        select @this) {
                    if (word.Trim().Length > 0) {
                        yield return word;
                    }
                }
            }
        }
    }
}