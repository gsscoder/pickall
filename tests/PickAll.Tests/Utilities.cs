using PickAll;
using Bogus;

static class ResultInfoExtensions
{
    public static ResultInfo CloneWithIndex(this ResultInfo result, ushort index)
    {
        return new ResultInfo(
            result.Originator,
            index,
            result.Url,
            result.Description,
            result.Data);
    }
}

static class ResultInfoHelper
{
    public static ResultInfo OnlyDescription(string text)
    {
        return new Faker<ResultInfo>()
            .RuleFor(o => o.Originator, _ => "helper")
            .RuleFor(o => o.Index, _ => (ushort)0)
            .RuleFor(o => o.Url, f => f.Internet.UrlWithPath(fileExt: "php"))
            .RuleFor(o => o.Description, _ => text)
                .Generate();
    }
}