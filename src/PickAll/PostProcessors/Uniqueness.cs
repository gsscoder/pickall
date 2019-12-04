using System.Collections.Generic;

namespace PickAll.PostProcessors
{
    /// <summary>
    /// Post processor to remove duplicate results by URL. 
    /// </summary>
    public class Uniqueness : IPostProcessor
    {
        public IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
        {
            return results.DistinctBy(result => result.Url);
        }
    }
}