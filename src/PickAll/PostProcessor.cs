using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickAll
{
    /// <summary>
    /// Represents a post processor service managed by <see cref="SearchContext">.
    /// </summary>
    public abstract class PostProcessor
    {
        public PostProcessor(object settings)
        {
            Settings = settings;
        }

        protected object Settings
        {
            get;
            private set;
        }

        public abstract Task<IEnumerable<ResultInfo>> ProcessAsync(IEnumerable<ResultInfo> results);
    }
}