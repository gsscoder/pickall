using System.Collections.Generic;
using System.Linq;

namespace PickAll
{
    /// <summary>Orders results placing indexes of same number close by each other.</summary>
    public class Order : PostProcessor
    {
        public Order(object settings) : base(settings)
        {
        }

        public override IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
        {
            return results.OrderBy(result => result.Index);
        }
    }
}