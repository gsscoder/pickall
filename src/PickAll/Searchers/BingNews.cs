using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace PickAll
{
    /// <summary><c>Searcher</c> that searches on Bing search engine for news.</summary>
    public class BingNews : Searcher
    {
        const string _searchUrl = "https://www.bing.com/news";

        public BingNews(object settings) : base(settings)  
        {
        }

        public override async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            using var document = await Context.Browsing.OpenAsync(_searchUrl);
            var form = document.QuerySelector<IHtmlFormElement>("#sb_form");
            ((IHtmlInputElement)form["sb_form_q"]).Value = query;
            using var result = await form.SubmitAsync(form);
            // Select only actual results
            var links = from l in
                        (from link in result.QuerySelectorAll<IHtmlAnchorElement>("a[h*='news']")
                        where link.Href.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        select link)
                        where !l.Href.Contains("go.microsoft.com") &&
                              !l.Href.Equals($"{_searchUrl}/search#", StringComparison.OrdinalIgnoreCase) &&
                              !string.IsNullOrWhiteSpace(l.Text)
                        select l;

            return links.Select((link, index) =>
                CreateResult((ushort)index, link.Href, link.Text));
        }
    }
}
