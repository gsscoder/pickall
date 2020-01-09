using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace PickAll.Searchers
{
    /// <summary>
    /// <see cref="Searcher"> that searches on Bing search engine.
    /// </summary>
    public class Bing : Searcher
    {
        public Bing(object settings) : base(settings)  
        {
        }

        public override async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            using (var document = await Context.ActiveContext.OpenAsync("https://www.bing.com/")) {
                var form = document.QuerySelector<IHtmlFormElement>("#sb_form");
                ((IHtmlInputElement)form["sb_form_q"]).Value = query;
                using (var result = await form.SubmitAsync(form)) {
                    // Select only actual results
                    var links = from link in result.QuerySelectorAll<IHtmlAnchorElement>("li.b_algo a")
                                where link.Attributes["href"].Value.StartsWith("http",
                                    StringComparison.OrdinalIgnoreCase)
                                select link;

                    return links.Select((link, index) =>
                        CreateResult((ushort)index, link.Attributes["href"].Value, link.Text));
                }
            }
        }
    }
}