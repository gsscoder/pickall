using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using WaffleGenerator;
using AngleSharp;
using AngleSharp.Dom;
using CSharpx;
using PickAll;

static class ResultInfoBuilder
{
    public static IEnumerable<ResultInfo> Generate(string originator, ushort samples)
    {
        for (ushort index = 0; index <= samples - 1; index++) {
            var faker = new Faker<ResultInfo>()
                .RuleFor(o => o.Originator, _ => originator)
                .RuleFor(o => o.Index, _ => index)
                .RuleFor(o => o.Url, f => f.Internet.UrlWithPath(fileExt: "html"))
                .RuleFor(o => o.Description, f => f.WaffleTitle());
            yield return faker.Generate();
        } 
    }

    public static IEnumerable<ResultInfo> GenerateRandom(string originator, ushort minSamples, ushort maxSamples)
    {
        var samples = new CryptoRandom().Next(minSamples, maxSamples);
        for (ushort index = 0; index <= samples - 1; index++) {
            var faker = new Faker<ResultInfo>()
                .RuleFor(o => o.Originator, _ => originator)
                .RuleFor(o => o.Index, _ => index)
                .RuleFor(o => o.Url, f => f.Internet.UrlWithPath(fileExt: "html"))
                .RuleFor(o => o.Description, f => f.WaffleTitle());
            yield return faker.Generate();
        } 
    }

    public static IEnumerable<ResultInfo> GenerateUnique(string originator, ushort samples)
    {
        var generated = new List<ResultInfo>();
        for (ushort index = 0; index <= samples - 1; index++) {
            var faker = new Faker<ResultInfo>()
                .RuleFor(o => o.Originator, _ => originator)
                .RuleFor(o => o.Index, _ => index)
                .RuleFor(o => o.Url, f => f.Internet.UrlWithPath(fileExt: "html"))
                .RuleFor(o => o.Description, f => f.WaffleTitle());
            var candidate = faker.Generate();
            var searched = from @this in generated
                            where @this.Url == candidate.Url || @this.Description == candidate.Description
                            select @this;
            if (searched.Count() == 0) {
                generated.Add(candidate);
            }
            else {
                index--;
            }
        }
        return generated;
    }
}

static class WaffleBuilder
{
    public static IEnumerable<string> GenerateTitle(int times, Func<string, string> modifier = null)
    {
        Func<string, string> _nullModifier = @string => @string;
        var _modifier = modifier ?? _nullModifier;

        for (var i = 0; i < times; i++) {
            var title = WaffleEngine.Title();
            yield return _modifier(title);
        }
    }

    public static IDocument GeneratePage(int paragraphs = 1)
    {
        var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
        return context.OpenAsync(request => request.Content(
                WaffleEngine.Html(
                    paragraphs: paragraphs,
                    includeHeading: true,
                    includeHeadAndBody: true))).GetAwaiter().GetResult();
    } 
}