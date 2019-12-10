using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp;

namespace PickAll
{
    /// <summary>
    /// Represents a searching services managed by <see cref="SearchContext">.
    /// </summary>
    public abstract class Searcher
    {
        public Searcher(IBrowsingContext context, object settings)
        {
            Context = context;
            Settings = settings;
            Name = GetType().Name;
        }

        protected IBrowsingContext Context
        {
            get;
            private set;
        }

        protected object Settings
        {
            get;
            private set;
        }

        /// <summary>
        /// Performs the actual search.
        /// </summary>
        /// <param name="query">A query string.</param>
        /// <returns>A collection of <see cref="ResultInfo"> with search results.</returns>
        public abstract Task<IEnumerable<ResultInfo>> SearchAsync(string query);

        /// <summary>
        /// The searcher identifier set to class name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        protected ResultInfo CreateResult(
            ushort index, string url, string description, object data = null)
        {
            return new ResultInfo(Name, index, url, description, data);
        }
    }
}