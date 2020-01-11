using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using AngleSharp;
using AngleSharp.Io.Network;
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
        private Lazy<IBrowsingContext> _activeContext;
        private static readonly Lazy<SearchContext> _default = new Lazy<SearchContext>(
            () => new SearchContext(
                typeof(Google),
                typeof(DuckDuckGo),
                typeof(Uniqueness),
                typeof(Order)));

        internal SearchContext(IEnumerable<Service> services, ContextSettings settings)
        {
            Services = services;
            Settings = settings;
            _activeContext = new Lazy<IBrowsingContext>(() => BuildContext(settings));
#if DEBUG
            DebugEnforceMaximumResults = true;
#endif
        }

        public SearchContext(ContextSettings settings): this(Enumerable.Empty<Service>(), settings)
        {
        }

        public SearchContext() : this(Enumerable.Empty<Service>(), new ContextSettings())
        {
        }

        public SearchContext(uint maximumResults)
            : this(Enumerable.Empty<Service>(), new ContextSettings { MaximumResults = maximumResults })
        {
        }

        public SearchContext(TimeSpan timeout)
            : this(Enumerable.Empty<Service>(), new ContextSettings { Timeout = timeout })
        {
        }

        /// <summary>
        /// Builds a new search context with a given types.
        /// </summary>
        /// <param name="services">A list of service types.</param>
        public SearchContext(params Type[] services) : this()
        {
            var servicesCount = (from service in services
                                 where service.IsService()
                                 select service).Count();
            if (servicesCount < services.Count()) {
                throw new NotSupportedException(
                    "All types must inherit from Searcher or PostProcessor");
            }

            Services = (from service in services
                        select service.IsSearcher()
                               ? Activator.CreateInstance(service, new object[] { null, new RuntimePolicy() })
                               : Activator.CreateInstance(service, new object[] { null }))
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

#if !DEBUG
        internal ContextSettings Settings { get; private set; }

        internal IEnumerable<Service> Services { get; private set; }
#else
        public ContextSettings Settings { get; private set; }

        public IEnumerable<Service> Services { get; private set; }

        public bool DebugEnforceMaximumResults { get; set; }
#endif

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
                       select BindContext(this, service);

            // Invoke searchers in parallel
            var resultGroup = await Task.WhenAll(
                from searcher in 
                    (from service in Services
                     where service.GetType().IsSearcher()
                     select service).Cast<Searcher>()
                select searcher.SearchAsync(query));
            var results = resultGroup.SelectMany(group => group).ToList();

            if (Settings.MaximumResults != null) {
#if !DEBUG
                results = new List<ResultInfo>(results.Take((int)Settings.MaximumResults.Value));
#else
                if (DebugEnforceMaximumResults) {
                    results = new List<ResultInfo>(results.Take((int)Settings.MaximumResults.Value));
                }
#endif
            }

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

        static Service BindContext(SearchContext context, Service service)
        {
            service.Context = context;
            return service;
        }

        static IBrowsingContext BuildContext(ContextSettings settings)
        {
            if (settings.Timeout.HasValue) {
                var client = new HttpClient();
                client.Timeout =  settings.Timeout.Value;
                var requester = new HttpClientRequester();
                return BrowsingContext.New(
                    Configuration.Default
                        .WithRequester(requester)
                        .WithDefaultLoader());
            }
            return BrowsingContext.New(Configuration.Default.WithDefaultLoader());
        }
    }
}