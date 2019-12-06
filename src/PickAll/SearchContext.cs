using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp;
using PickAll.Searchers;
using PickAll.PostProcessors;

namespace PickAll
{
    /// <summary>
    /// Manages <see cref="Searcher"> and <see cref="IPostProcessor"> instances to gather and
    /// elaborate results.
    /// </summary>
    public sealed class SearchContext
    {   
        private readonly IEnumerable<object> _services;
        private static readonly Lazy<IBrowsingContext> _activeContext = new Lazy<IBrowsingContext>(
            () => BrowsingContext.New(Configuration.Default.WithDefaultLoader()));
        private static readonly Lazy<SearchContext> _defaultContext = new Lazy<SearchContext>(
            () => new SearchContext(new object[]
                {
                    new Google(),
                    new DuckDuckGo(),
                    new Uniqueness(),
                    new Order()
                }));

        public SearchContext(IEnumerable<object> services = null)
        {
            _services = services ?? new object[] {};
        }

#if DEBUG
        public IEnumerable<object> Services
#else
        internal IEnumerable<object> Services
#endif
        {
            get { return _services; }
        }


        internal static IBrowsingContext ActiveContext
        {
            get { return _activeContext.Value; }
        }

        /// <summary>
        /// Executes a search asynchronously, invoking all <see cref="Searcher">
        /// and <see cref="IPostProcessor"> services.
        /// </summary>
        /// <param name="query">A query string for sercher services.</param>
        /// <returns>A colection of <see cref="ResultInfo">.</returns>
        public async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query),
                $"{nameof(query)} cannot be null");
            if (query.Trim() == string.Empty) throw new ArgumentException(nameof(query),
                $"{nameof(query)} cannot be empty or contains only white spaces");

            var results = new List<ResultInfo>();
            foreach (var service in Services) {
                if (service.GetType().IsSubclassOf(typeof(Searcher))) {
                    results.AddRange(await ((Searcher)service).SearchAsync(query));
                } else if (typeof(IPostProcessor).IsAssignableFrom(service.GetType())) {
                    var current = await ((IPostProcessor)service).ProcessAsync(results);
                    results = new List<ResultInfo>();
                    results.AddRange(current);
                }
            }
            return results;
        }

        /// <summary>
        /// Gets the <see cref="SearchContext"> instance created with default services.
        /// </summary>
        public static SearchContext Default
        {
            get { return _defaultContext.Value; }
        }
    }
}