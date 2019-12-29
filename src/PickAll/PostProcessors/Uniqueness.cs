using System.Collections.Generic;
using PickAll.Internal;

namespace PickAll.PostProcessors
{
    /// <summary>
    /// Removes duplicate results by URL. 
    /// </summary>
    public class Uniqueness : PostProcessor
    {
        public Uniqueness(object settings) : base(settings)
        {
        }

        public override IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
        {
            return results.DistinctBy(result => result.Url);
        }
    }
}