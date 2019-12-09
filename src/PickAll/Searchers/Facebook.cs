using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace PickAll.Searchers
{
    /// <summary>
    /// <see cref="Searcher"> that searches on Google search engine.
    /// </summary>
    public class Facebook : Searcher
    {
        public class Options
        {
            public bool RetrieveImageLink;
        }

        public class Data
        {
            public Data(string imageUrl)
            {
                ImageUrl = imageUrl;
            }

            public string ImageUrl { get; private set; }

            public override string ToString()
            {
                return $"ImageUrl: {ImageUrl}";
            }
        }

        private static string _baseUrl = "https://m.facebook.com";
        private readonly Options _options;

        public Facebook(Options options) : base()  
        {
            _options = options;
        }

        public Facebook() : this(new Options { RetrieveImageLink = false })
        {
        }

        public override async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            var formatted = string.Join("+", query.Split());
            using (var document = await Context.OpenAsync($"{_baseUrl}/public/{formatted}")) {
                // Results are list of tables
                var tables = document.QuerySelectorAll<IHtmlTableElement>(
                    "div#BrowseResultsContainer table");
                var results = new List<ResultInfo>();
                ushort index = 0;
                foreach (var table in tables) {
                    var link =table.QuerySelector<IHtmlAnchorElement>("td.bt.bu a")
                        .Attributes["href"].Value;
                    var description = table.QuerySelector<IHtmlDivElement>("div.bw").Text();
                    object data = null;
                    if (_options.RetrieveImageLink)
                    {
                        var imageLink = table.QuerySelector<IHtmlImageElement>("td.bo.bp img")
                            .Attributes["src"].Value;
                        data = new Data(imageLink);
                    }
                    results.Add(CreateResult(index, $"{_baseUrl}{link}", description, data));
                    index++;
                }
                return results;
            }
        }

        private static string GetLinkFromTitle(IHtmlDivElement title) {
            var anchor = title.Parent.Parent as IHtmlAnchorElement;
            if (anchor == null) {
                return string.Empty;
            }
            var href = anchor.Attributes["href"].Value;
            return $"{_baseUrl}{href}";
        }
    }
}