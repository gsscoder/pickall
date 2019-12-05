using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickAll
{
    /// <summary>
    /// Represents a post processor service managed by <see cref="SearchContext">.
    /// </summary>
    public interface IPostProcessor
    {
        Task<IEnumerable<ResultInfo>> ProcessAsync(IEnumerable<ResultInfo> results);
    }
}