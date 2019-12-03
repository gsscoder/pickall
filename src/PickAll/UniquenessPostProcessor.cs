using System.Collections.Generic;

namespace PickAll
{
    /// <summary>
    /// Post processor to remove duplicate results by URL. 
    /// </summary>
    public class UniquenessPostProcessor : IPostProcessor
    {
        public IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
        {
            return results.DistinctBy(result => result.Url);
        }
    }
}