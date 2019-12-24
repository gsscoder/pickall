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
    /// Manages <see cref="Searcher"/> and <see cref="PostProcessor"/> instances to gather and
    /// elaborate results.
    /// </summary>
    public sealed class SearchContext
    {   
        private static readonly Lazy<IBrowsingContext> _activeContext = new Lazy<IBrowsingContext>(
            () => BrowsingContext.New(Configuration.Default.WithDefaultLoader()));
        private static readonly Lazy<SearchContext> _default = new Lazy<SearchContext>(
            () => new SearchContext(
                typeof(Google),
                typeof(DuckDuckGo),
                typeof(Uniqueness),
                typeof(Order)));

        internal SearchContext(IEnumerable<Service> services)
        {
            Services = services;
        }

        public SearchContext() : this(Enumerable.Empty<Service>())
        {
        }

        /// <summary>
        /// Builds a new search context with a given types.
        /// </summary>
        /// <param name="services">A list of service types.</param>
        public SearchContext(params Type[] services)
        {
            var servicesCount = (from service in services
                                 where service.IsService()
                                 select service).Count();
            if (servicesCount < services.Count()) {
                throw new NotSupportedException(
                    "All types must inherit from Searcher or PostProcessor");
            }

            Services = (from service in services
                        select Activator.CreateInstance(service, new object[] { null }))
                        .Cast<Service>();
        }

        public IBrowsingContext ActiveContext
        {
            get { return _activeContext.Value; }
        }

        public string Query
        {
            get;
            private set;
        }

        internal IEnumerable<Service> Services
        {
            get; private set;
        }

        /// <summary>
        /// Executes a search asynchronously, invoking all <see cref="Searcher"/>
        /// and <see cref="PostProcessor"/> services.
        /// </summary>
        /// <param name="query">A query string for sercher services.</param>
        /// <returns>A colection of <see cref="ResultInfo"/>.</returns>
        public async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query),
                $"{nameof(query)} cannot be null");
            if (query.Trim() == string.Empty) throw new ArgumentException(
                $"{nameof(query)} cannot be empty or contains only white spaces", nameof(query));

            Query = query;
            
            Services = from service in Services
                       select BindContext(service);

            // Invoke searchers in parallel
            var resultGroup = await Task.WhenAll(
                from searcher in 
                    (from service in Services
                     where service.GetType().IsSearcher()
                     select service).Cast<Searcher>()
                select searcher.SearchAsync(query));
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
        /// Builds a <see cref="SearchContext"/> instance with default services.
        /// </summary>
        public static SearchContext Default
        {
            get { return _default.Value; }
        }

        Service BindContext(Service service)
        {
            service.Context = this;
            return service;
        }
    }
}