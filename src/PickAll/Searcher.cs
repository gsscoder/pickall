using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickAll
{
    /// <summary>
    /// Represents a searching service managed by <see cref="SearchContext"/>.
    /// </summary>
    public abstract class Searcher : Service
    {
        public Searcher(object settings)
        {
            Settings = settings;
            Name = GetType().Name;
        }

        internal event EventHandler<ResultHandledEventArgs> ResultCreated;

        /// <summary>
        /// The searcher identifier set to class name.
        /// </summary>
        public string Name { get; private set; }

        public RuntimePolicy Policy { get; internal set; }

        /// <summary>
        /// Performs the actual search.
        /// </summary>
        /// <param name="query">A query string.</param>
        /// <returns>A collection of <see cref="ResultInfo"/> with search results.</returns>
        public abstract Task<IEnumerable<ResultInfo>> SearchAsync(string query);

        protected ResultInfo CreateResult(
            int index, string url, string description, object data = null)
        {
            var result = new ResultInfo(Name, index, url, description, data);
            EventHelper.RaiseEvent(this, ResultCreated,
                () => new ResultHandledEventArgs(result, ServiceType.Searcher), Context.Settings.EnableRaisingEvents);
            return result;
        }
    }
}