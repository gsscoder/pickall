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
        public SearchContext()
        {
            ActiveContext = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            Services = new object[] {};
        }

#if DEBUG
        public IEnumerable<object> Services
#else
        internal IEnumerable<object> Services
#endif
        {
            get; set;
        }

        internal IBrowsingContext ActiveContext
        {
            get; private set;
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
        /// Builds a <see cref="SearchContext"> instance, registering default services.
        /// </summary>
        /// <returns>A <see cref="SearchContext"> instance.</returns>
        public static SearchContext Default()
        {
            var context = new SearchContext();
            context.Services = new object[]
                {
                    SetUpService(new Google(), context.ActiveContext),
                    SetUpService(new DuckDuckGo(), context.ActiveContext),
                    SetUpService(new Uniqueness()),
                    SetUpService(new Order())
                };
            return context;
        }

        private static object SetUpService(object service, IBrowsingContext context = null)
        {
            var configured = service;
            if (context != null) {
                ((Searcher)service).Context = context;
            }
            return configured;
        }
    }
}