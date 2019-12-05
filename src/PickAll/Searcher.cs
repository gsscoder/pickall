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
        private readonly string _name;
        private IBrowsingContext _context;

        protected Searcher()
        {
            _name = GetType().Name;
        }

        public IBrowsingContext Context
        {
            get { return _context; }
            
            set {
                if (_context != null) {
                    throw new InvalidOperationException("Context cannot be set more the once");
                }
                _context = value;
            }
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