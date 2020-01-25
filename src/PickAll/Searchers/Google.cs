using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace PickAll
{
    /// <summary>
    /// <see cref="Searcher"/> that searches on Google search engine.
    /// </summary>
    public class Google : Searcher
    {
        static readonly Regex _expression = new Regex(@"^/url\?q=([^&]*)&.*",
            RegexOptions.Compiled);

        public Google(object settings) : base(settings)  
        {
        }

        public override async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            using (var document = await Context.Browsing.OpenAsync("https://www.google.com/")) {
                var form = document.QuerySelector<IHtmlFormElement>("form[action='/search']");
                using (var result = await form.SubmitAsync(
                    new { q = query })) {
                    // Take only valid URLs
                    var links = from anchor in result.QuerySelectorAll<IHtmlAnchorElement>("a")
                                where Validate(anchor.Attributes["href"].Value)
                                select anchor;
                    // Create results normalizing URLs
                    var results = links.Select((link, index) =>
                        CreateResult((ushort)index, Normalize(link.Attributes["href"].Value),
                            link.FirstChildText("div", "span")));
                    // Discard ones without description (not actual results)
                    return from @this in results
                            where @this.Description.Trim() != string.Empty
                            select @this;
                }
            }
        }

        private static bool Validate(string url)
        {
            return
                url.StartsWith(
                    "/url?", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith(
                    "/url?q=http://webcache.googleusercontent.com",StringComparison.OrdinalIgnoreCase);
        }

        private static string Normalize(string url)
        {
            var match = _expression.Match(url);
            return match.Groups.Count == 2 ? match.Groups[1].Value : url;
        }
    }
}