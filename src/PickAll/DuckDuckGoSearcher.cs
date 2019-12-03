using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace PickAll
{
    /// <summary>
    /// <see cref="Searcher"> that searches on DuckDuckGo search engine.
    /// </summary>
    public class DuckDuckGoSearcher : Searcher
    {
        public DuckDuckGoSearcher(IBrowsingContext context) : base(context)  
        {
        }

        public override async Task<IEnumerable<ResultInfo>> Search(string query)
        {
            using (var document = await Context.OpenAsync("https://duckduckgo.com/")) {
                var form = document.QuerySelector<IHtmlFormElement>("#search_form_homepage");
                using (var result = await form.SubmitAsync(
                    new { q = query })) {
                    var links = result.QuerySelectorAll<IHtmlAnchorElement>("a.result__a");

                    return links.Select((link, index) =>
                        CreateResult((ushort)index, link.Attributes["href"].Value, link.Text));
                }
            }
        }

        public override string Name
        {
            get { return "duckduckgo"; }
        }
    }
}