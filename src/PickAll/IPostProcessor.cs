using System.Collections.Generic;

namespace PickAll
{
    /// <summary>
    /// Represents a post processor service managed by <see cref="SearchContext">.
    /// </summary>
    public interface IPostProcessor
    {
        IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results);
    }
}