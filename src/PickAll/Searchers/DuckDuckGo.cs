using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace PickAll.Searchers
{
    /// <summary>
    /// <see cref="Searcher"> that searches on DuckDuckGo search engine.
    /// </summary>
    public class DuckDuckGo : Searcher
    {
        public DuckDuckGo(object settings) : base(settings)  
        {
        }

        public override async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            using (var document = await Context.ActiveContext.OpenAsync("https://duckduckgo.com/")) {
                var form = document.QuerySelector<IHtmlFormElement>("#search_form_homepage");
                using (var result = await form.SubmitAsync(
                    new { q = query })) {
                    var links = result.QuerySelectorAll<IHtmlAnchorElement>("a.result__a");

                    return links.Select((link, index) =>
                        CreateResult((ushort)index, link.Attributes["href"].Value, link.Text));
                }
            }
        }
    }
}