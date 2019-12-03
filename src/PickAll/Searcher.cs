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

        protected Searcher(IBrowsingContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context),
                $"{nameof(context)} cannot be null");

            _context = context;
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
        public abstract Task<IEnumerable<ResultInfo>> Search(string query);

        /// <summary>
        /// The searcher identifier.
        /// </summary>
        public abstract string Name { get; }

        protected ResultInfo CreateResult(ushort index, string url, string description)
        {
            return new ResultInfo(Name, index, url, description);
        }
    }
}