using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickAll.PostProcessors
{
    /// <summary>
    /// Post processor to order results placing indexes of same number close by each other.
    /// </summary>
    public class Order : PostProcessor
    {
        public Order(object settings = null) : base(settings)
        {
        }

        public override async Task<IEnumerable<ResultInfo>> ProcessAsync(IEnumerable<ResultInfo> results)
        {
            return await Task.Run(() =>results.OrderBy(result => result.Index));
        }
    }
}