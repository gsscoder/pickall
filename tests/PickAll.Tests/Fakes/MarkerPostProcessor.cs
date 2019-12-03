using System.Collections.Generic;

namespace PickAll.Tests.Fakes
{
    public class MarkPostProcessor : IPostProcessor
    {
        private readonly string _stamp;

        public MarkPostProcessor(string stamp)
        {
            _stamp = stamp;
        }

        public IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
        {
            foreach (var result in results) {
                yield return new ResultInfo(result.Originator, result.Index, result.Url,
                    $"{_stamp}|{result.Description}");
            }
        }
    }
}