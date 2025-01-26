using System.Text.RegularExpressions;
using HtmlAgilityPack;
using PickAll.Searchers;
using PuppeteerSharp;
using SharpX.Extensions;

namespace PickAll;

/// <summary><c>Searcher</c> that searches on Google search engine.</summary>
public class Google : Searcher
{
    static readonly Regex _normalize = new Regex(@"^/url\?q=([^&]*)&.*", RegexOptions.Compiled);

    public Google(object settings) : base(settings)  
    {
    }

    public override async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
    {
        var page = await Context.HeadlessBrowsing.NewPageAsync();
        await page.UseStealthMode();
        var url = $"https://www.google.com/search?q={Uri.EscapeDataString(query)}";
        var response = await page.GoToAsync(url, new NavigationOptions
        {
            WaitUntil = [WaitUntilNavigation.Load]
        });
        response.EnsureSuccessOrThrow(
            new SearcherException("Unable to navigate to 'https://www.google.com/search?q={query}'."));

        var htmlContent = await page.EvaluateFunctionAsync<string>(
            "document.querySelector('#center_col').outerHTML");
        if (htmlContent == null) {
            throw new SearcherException($"Unable to select item 'center_col'.");
        }
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        var links = htmlDoc.DocumentNode.SelectNodes("//a")
            .Where(x => !x.GetAttributeValue("href", string.Empty).IsEmpty());

        //using var result = await form.SubmitAsync(new { q = query });
        //// Take only valid URLs
        //var links = from anchor in result.QuerySelectorAll<IHtmlAnchorElement>("a")
        //            where Validate(anchor.Attributes["href"].Value)
        //            select anchor;
        //// Create results normalizing URLs
        //var results = links.Select((link, index) =>
        //    CreateResult((ushort)index, Normalize(link.Attributes["href"].Value),
        //        link.FirstChildText("div", "span")));

        //// Discard ones without description (not actual results)
        //return from @this in results
        //       where @this.Description.Trim() != string.Empty
        //       select @this;

        return Enumerable.Empty<ResultInfo>();
    }

    static bool Validate(string url) =>
            url.StartsWith(
                "/url?", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith(
                "/url?q=http://webcache.googleusercontent.com",StringComparison.OrdinalIgnoreCase);

    static string Normalize(string url)
    {
        var match = _normalize.Match(url);
        return match.Groups.Count == 2 ? match.Groups[1].Value : url;
    }
}
