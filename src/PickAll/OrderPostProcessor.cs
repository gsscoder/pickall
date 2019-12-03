using System.Collections.Generic;
using System.Linq;

namespace PickAll
{
    /// <summary>
    /// Post processor to order results placing indexes of same number close by each other.
    /// </summary>
    public class OrderPostProcessor : IPostProcessor
    {
        public IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
        {
            return results.OrderBy(result => result.Index);
        }
    }
}