using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickAll.PostProcessors
{
    /// <summary>
    /// Post processor to order results placing indexes of same number close by each other.
    /// </summary>
    public class Order : IPostProcessor
    {
        public async Task<IEnumerable<ResultInfo>> ProcessAsync(IEnumerable<ResultInfo> results)
        {
            return results.OrderBy(result => result.Index);
        }
    }
}