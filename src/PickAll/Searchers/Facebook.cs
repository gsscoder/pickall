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
        private static string _baseUrl = "https://m.facebook.com";

        public Facebook() : base()  
        {
        }

        public override async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            var formatted = string.Join("+", query.Split());
            using (var document = await Context.OpenAsync($"{_baseUrl}/public/{formatted}")) {
                // Select titles to focus on actual results
                var titles = document.QuerySelectorAll<IHtmlDivElement>(
                    "div#BrowseResultsContainer div.bw");

                return titles.Select((title, index) =>
                    CreateResult((ushort)index, GetLinkFromTitle(title), title.Text()));
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