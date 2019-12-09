using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickAll.Tests.Fakes
{
    public class MarkPostProcessor : IPostProcessor
    {
        private readonly string _stamp;

        public MarkPostProcessor(string stamp)
        {
            _stamp = stamp;
        }

        public async Task<IEnumerable<ResultInfo>> ProcessAsync(IEnumerable<ResultInfo> results)
        {
            return await Task.Run(() => _());
            IEnumerable<ResultInfo> _() {
                foreach (var result in results) {
                    yield return new ResultInfo(result.Originator, result.Index, result.Url,
                        $"{_stamp}|{result.Description}", null);
                }
            }
        }
    }
}