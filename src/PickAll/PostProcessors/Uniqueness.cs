using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickAll.PostProcessors
{
    /// <summary>
    /// Post processor to remove duplicate results by URL. 
    /// </summary>
    public class Uniqueness : PostProcessor
    {
        public Uniqueness(object settings = null) : base(settings)
        {
        }

        public override IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
        {
            return results.DistinctBy(result => result.Url);
        }
    }
}