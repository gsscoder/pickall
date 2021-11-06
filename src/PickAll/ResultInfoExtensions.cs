namespace PickAll
{
    public static class ResultInfoExtensions
    {
        public static ResultInfo Clone(this ResultInfo resultInfo, object data = null)
        {
            Guard.AgainstNull(nameof(resultInfo), resultInfo);

            var _data = data ?? resultInfo.Data;
            return new ResultInfo(
                resultInfo.Originator,
                resultInfo.Index,
                resultInfo.Url,
                resultInfo.Description,
                _data);
        }
    }
}
