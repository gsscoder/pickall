using System;
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
        private readonly IBrowsingContext _context;
        private readonly string _name;

        protected Searcher()
        {
            _context = SearchContext.ActiveContext;
            _name = GetType().Name;
        }

        protected IBrowsingContext Context
        {
            get { return _context; }
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
            get { return _name; }
        }

        protected ResultInfo CreateResult(ushort index, string url, string description)
        {
            return new ResultInfo(Name, index, url, description);
        }
    }
}