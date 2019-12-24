namespace PickAll.Tests
{
    public static class ResultInfoHelper
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
}