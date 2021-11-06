using PickAll;
using Bogus.DataSets;

static class ResultInfoHelper
{
    static readonly Internet _internet = new Internet();

    public static ResultInfo OnlyDescription(string text)
    {
        return new ResultInfo(
            originator: "helper",
            index: 0,
            url: _internet.UrlWithPath(fileExt: "php"),
            description: text,
            data: null);
    }
}
