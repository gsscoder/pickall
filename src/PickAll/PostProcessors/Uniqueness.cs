using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickAll.PostProcessors
{
    /// <summary>
    /// Post processor to remove duplicate results by URL. 
    /// </summary>
    public class Uniqueness : IPostProcessor
    {
        public async Task<IEnumerable<ResultInfo>> ProcessAsync(IEnumerable<ResultInfo> results)
        {
            return results.DistinctBy(result => result.Url);
        }
    }
}