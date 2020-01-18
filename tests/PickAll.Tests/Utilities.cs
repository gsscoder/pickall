static class ResultInfoExtensions
{
    public static ResultInfo UsingIndex(this ResultInfo result, ushort index)
    {
        return new ResultInfo(
            result.Originator,
            index, result.Url,
            result.Description,
            result.Data);
    }
}

static class ResultInfoHelper
{
    public static ResultInfo OnlyDescription(string text)
    {
        return new ResultInfo(
            "helper",
            0,
            "http://fake-url.com/",
            text,
            null);
    }
}