using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickAll
{
    /// <summary>
    /// Represents a post processor service managed by <see cref="SearchContext">.
    /// </summary>
    public abstract class PostProcessor : IService
    {
        public PostProcessor(object settings)
        {
            Settings = settings;
        }

        public ContextState State
        {
            get; set;
        }

        protected object Settings
        {
            get;
            private set;
        }

        public abstract IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results);
    }
}