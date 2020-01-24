using PickAll;
using Bogus;

static class ResultInfoHelper
{
    public static ResultInfo OnlyDescription(string text)
    {
        return new Faker<ResultInfo>()
            .RuleFor(o => o.Originator, _ => "helper")
            .RuleFor(o => o.Index, _ => 0)
            .RuleFor(o => o.Url, f => f.Internet.UrlWithPath(fileExt: "php"))
            .RuleFor(o => o.Description, _ => text)
                .Generate();
    }
}