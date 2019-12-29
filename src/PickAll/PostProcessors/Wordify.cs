using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using CSharpx;
using PickAll.Internal;

namespace PickAll.PostProcessors
{
    /// <summary>
    /// Settings for <see cref="Wordify"/> post processor.
    /// </summary>
    public class WordifySettings
    {
        /// <summary>
        /// If set to true, page title will be included in result.
        /// </summary>
        public bool IncludeTitle;

        /// <summary>
        /// Maximum allowed length of page to scrape. If null, will be to to a default
        /// of 100000.
        /// </summary>
        /// <remarks>
        /// An high limit with numerous pages to scrape can be resource intensive.
        /// </remarks>
        public uint? MaximumLength;
    }

    /// <summary>
    /// Data produced by <see cref="Wordify"/> post processor.
    /// </summary>
    public class WordifyData
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
            _settings = settings as WordifySettings;
            if (_settings == null) {
                throw new NotSupportedException(
                    $"{nameof(settings)} must be of {nameof(WordifySettings)} type");
            }
        }

        public override IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
        {
            var limit = _settings.MaximumLength ?? 100000;
            foreach (var result in results) {
                using (var document = Context.ActiveContext.OpenAsync(result.Url)
                    .GetAwaiter().GetResult())
                {
                    if (document.ToHtml().Length <= limit) {
                        yield return result.Clone(new WordifyData(ExtractText(document)));
                    }
                    else {
                        yield return result;
                    }
                }
            }
        }

#if DEBUG
        internal
#endif
        IEnumerable<string> ExtractText(IDocument document)
        {
            var result = new List<String>();
            if (_settings.IncludeTitle) {
                result.AddRange(document.Title.Split());
            }
            result.AddRange(TextFromDocument());
            return result.Distinct();

            IEnumerable<string> TextFromDocument()
            {
                var elements =  document.SelectElements(
                    "div", "p", "h1", "h2","h3", "h4", "h5", "h6");
                foreach (var element in elements) {
                    var words = element.Text().Sanitize(normalizeWhiteSpace: true).Split();
                    foreach (var word in words) {
                        if (word.Trim().Length > 0) {
                            yield return word;
                        }
                    }
                }
            }
        }


    }
}