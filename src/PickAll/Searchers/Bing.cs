using System.Data;
using HtmlAgilityPack;
using PickAll.Searchers;
using PuppeteerSharp;
using SharpX.Extensions;

namespace PickAll;

/// <summary><c>Searcher</c> that searches on Bing search engine.</summary>
public class Bing(object settings) : Searcher(settings)
{
    public override async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
    {
        var page = await Context!.HeadlessBrowsing.NewPageAsync();
        await page.UseStealthMode();
        var url = $"https://www.bing.com/search?q={Uri.EscapeDataString(query)}";
        var response = await page.GoToAsync(url, new NavigationOptions
        {
            WaitUntil = [WaitUntilNavigation.Load]
        });
        response.EnsureSuccessOrThrow(
            new SearcherException("Unable to navigate to 'https://www.bing.com/search?q={query}'."));

        var olHtmlContent = await page.EvaluateExpressionAsync<string>("document.querySelector('ol#b_results').outerHTML");
        if (olHtmlContent == null) {
            throw new SearcherException($"Unable to select item 'b_results'.");
        }
        var resultsHtml = new HtmlDocument();
        resultsHtml.LoadHtml(olHtmlContent);

        var links = resultsHtml.DocumentNode.SelectNodes("//li[@class='b_algo']//a")
            .Where(x => !x.InnerText.IsEmpty() && !x.InnerText.EqualsIgnoreCase("div") &&
                   !x.Attributes["href"].Value.ContainsIgnoreCase("javascript:"));

        var results = links.Select((link, index) =>
            CreateResult((ushort)index, link.Attributes["href"].Value, link.InnerText));
        return results;
    }
}
