using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace PickAll.Searchers
{
    /// <summary>
    /// <see cref="Searcher"> that searches on Yahoo search engine.
    /// </summary>
    public class Yahoo : Searcher
    {
        public Yahoo() : base()  
        {
        }

        public override async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            using (var document = await Context.OpenAsync("https://yahoo.com/")) {
                var form = document.QuerySelector<IHtmlFormElement>("#uh-search-form");
                ((IHtmlInputElement)form["uh-search-box"]).Value = query;
                using (var result = await form.SubmitAsync(form)) {
                    var links = result.QuerySelectorAll<IHtmlAnchorElement>("#web a");

                    return links.Select((link, index) =>
                        CreateResult((ushort)index, link.Attributes["href"].Value, link.Text));
                }
            }
        }
    }
}