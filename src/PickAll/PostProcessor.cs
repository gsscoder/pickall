using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickAll
{
    /// <summary>
    /// Represents a post processor service managed by <see cref="SearchContext"/>.
    /// </summary>
    public abstract class PostProcessor
    {
        public PostProcessor(SearchContext context, object settings)
        {
            Context = context;
            Settings = settings;
        }

        internal SearchContext Context
        {
            get;
            set;
        }

        protected object Settings
        {
            get;
            set;
        }

        public abstract IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results);
    }
}