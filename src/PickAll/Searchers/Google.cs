using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace PickAll.Searchers
{
    /// <summary>
    /// <see cref="Searcher"> that searches on Google search engine.
    /// </summary>
    public class Google : Searcher
    {
        private static readonly Regex _normalizeUrlRegEx = new Regex(@"^/url\?q=([^&]*)&.*",
            RegexOptions.Compiled);

        public Google() : base()  
        {
        }

        public override async Task<IEnumerable<ResultInfo>> Search(string query)
        {
            using (var document = await Context.OpenAsync("https://www.google.com/")) {
                var form = document.QuerySelector<IHtmlFormElement>("form[action='/search']");
                using (var result = await form.SubmitAsync(
                    new { q = query })) {
                    var links = result.QuerySelectorAll<IHtmlAnchorElement>("a").Where(
                        a => Validate(a.GetAttribute("href")));

                    return links.Select((link, index) =>
                        CreateResult((ushort)index, Normalize(link.Attributes["href"].Value),
                            link.FirstChildText("div", "span")));
                }
            }
        }

        private static bool Validate(string url)
        {
            return url.StartsWith("/url?") &&
                !url.StartsWith("/url?q=http://webcache.googleusercontent.com");
        }

        private static string Normalize(string url)
        {
            var match = _normalizeUrlRegEx.Match(url);
            return match.Groups.Count == 2 ? match.Groups[1].Value : url;
        }
    }
}