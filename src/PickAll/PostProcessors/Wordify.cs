using System;
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
        public bool IncludeTitle;
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
            foreach (var result in results)
            {
                yield return result.Clone(new WordifyData(
                    ExtractText(result).GetAwaiter().GetResult()));
            }
        }

        async Task<IEnumerable<string>> ExtractText(ResultInfo result)
        {
            var words = new List<String>();
            using (var document = await Context.ActiveContext.OpenAsync(result.Url))
            {
                if (_settings.IncludeTitle) {
                    words.AddRange(document.Title.Split());
                }
                words.AddRange(TextFromDocument(document));
            }
            return words;
        }

#if DEBUG
        internal
#endif
        IEnumerable<string> TextFromDocument(IDocument document)
        {
            return _().Distinct();
            IEnumerable<string> _() {
                var elements =  document.SelectElements(
                    "div", "p", "h1", "h2","h3", "h4", "h5", "h6");
                foreach (var element in elements) {
                    var words = element.Text().Sanitize(normalizeWhiteSpace: true).Split();
                    foreach (var word in words) {
                        yield return word;
                    }
                }
            };
        }
    }
}