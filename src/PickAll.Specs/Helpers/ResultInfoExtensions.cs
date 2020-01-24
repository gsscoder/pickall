using PickAll;

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