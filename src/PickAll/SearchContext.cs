using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using PickAll.Internal;
using PickAll.Searchers;
using PickAll.PostProcessors;

namespace PickAll
{
    /// <summary>
    /// Manages <see cref="Searcher"> and <see cref="PostProcessor"> instances to gather and
    /// elaborate results.
    /// </summary>
    public sealed class SearchContext
    {   
        private static readonly Lazy<IBrowsingContext> _activeContext = new Lazy<IBrowsingContext>(
            () => BrowsingContext.New(Configuration.Default.WithDefaultLoader()));
        private static readonly Lazy<SearchContext> _defaultContext = new Lazy<SearchContext>(
            () => new SearchContext(new object[]
                {
                    new Google(_activeContext.Value),
                    new DuckDuckGo(_activeContext.Value),
                    new Uniqueness(),
                    new Order()
                }));

        internal SearchContext(IEnumerable<object> services)
        {
            Services = services;
        }

        public SearchContext() : this(new object[] {})
        {
        }

        internal IEnumerable<object> Services
        {
            get; private set;
        }

        internal IBrowsingContext ActiveContext
        {
            get { return _activeContext.Value; }
        }

        /// <summary>
        /// Executes a search asynchronously, invoking all <see cref="Searcher">
        /// and <see cref="PostProcessor"> services.
        /// </summary>
        /// <param name="query">A query string for sercher services.</param>
        /// <returns>A colection of <see cref="ResultInfo">.</returns>
        public async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query),
                $"{nameof(query)} cannot be null");
            if (query.Trim() == string.Empty) throw new ArgumentException(
                $"{nameof(query)} cannot be empty or contains only white spaces", nameof(query));

            Services = BindContextState(Services, new ContextState(query));

            // Invoke searchers in parallel
            var searchers = (from service in Services
                             where service.GetType().IsSearcher()
                             select service).Cast<Searcher>();
            var resultGroup = await Task.WhenAll(
                searchers.Select(searcher => searcher.SearchAsync(query)));
            var results = resultGroup.SelectMany(group => group).ToList();
            // Invoke post processors in sync
            var processors = (from service in Services
                              where service.GetType().IsPostProcessor()
                              select service).Cast<PostProcessor>();
            foreach (var processor in processors) {
                var current = processor.Process(results);
                results = new List<ResultInfo>();
                results.AddRange(current);
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

        static IEnumerable<object> BindContextState(IEnumerable<object> services,
            ContextState state)
        {
            Func<IService, IService> bind = service =>
                {
                    var binded = service;
                    binded.State = state;
                    return binded;
                };
            return from service in services.Cast<IService>()
                   select bind(service);
        } 
    }
}