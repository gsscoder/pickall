using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickAll
{
    /// <summary>Represents a searching service managed by <c>SearchContext</c>.</summary>
    public abstract class Searcher : Service
    {
        public Searcher(object settings)
        {
            Settings = settings;
            Name = GetType().Name;
        }

        internal event EventHandler<ResultHandledEventArgs> ResultCreated;

        /// <summary>The searcher identifier set to class name.</summary>
        public string Name { get; private set; }

        /// <summary>Performs the actual search and returns a sequence of <c>ResultInfo</c>.</summary>
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