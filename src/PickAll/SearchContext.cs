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
        private static Type[] _types = { typeof(Searcher), typeof(PostProcessor) };
        private Lazy<IBrowsingContext> _activeContext;
        private static readonly Lazy<SearchContext> _default = new Lazy<SearchContext>(
            () => new SearchContext(
                typeof(Google),
                typeof(DuckDuckGo),
                typeof(Uniqueness),
                typeof(Order)));

        internal SearchContext(ServiceHost host, ContextSettings settings)
        {
            Host = host;
            Settings = settings;
            _activeContext = new Lazy<IBrowsingContext>(() => BuildContext(settings));
#if DEBUG
            EnforceMaximumResults = true;
#endif
        }

        public SearchContext(ContextSettings settings): this(ServiceHost.DefaultHost(_types), settings)
        {
        }

        public SearchContext() : this(ServiceHost.DefaultHost(_types), new ContextSettings())
        {
        }

        public SearchContext(uint maximumResults)
            : this(ServiceHost.DefaultHost(_types), new ContextSettings { MaximumResults = maximumResults })
        {
        }

        public SearchContext(TimeSpan timeout)
            : this(ServiceHost.DefaultHost(_types), new ContextSettings { Timeout = timeout })
        {
        }

        /// <summary>
        /// Builds a new search context with a given types.
        /// </summary>
        /// <param name="services">A list of service types.</param>
        public SearchContext(params Type[] services) : this()
        {
            Host = ServiceHost.DefaultHost(_types);
            foreach (var type in services) {
                Host = Host.Add(type, () => Activator.CreateInstance(type, new object[] { null }));
            }
        }

        public IBrowsingContext ActiveContext
        {
            get { return _activeContext.Value; }
        }

        public string Query
        {
            get; private set;
        }

        internal ServiceHost Host { get; private set; }

#if !DEBUG
        internal ContextSettings Settings { get; private set; }
#else
        public ContextSettings Settings { get; private set; }

        public IEnumerable<object> Services { get { return Host.Services; } } // Debug only

        public bool EnforceMaximumResults { get; set; } // Debug only
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

            // Bind context and partition maximum results
            Host = ConfigureServices(this);

            // Invoke searchers in parallel
            var resultGroup = await Task.WhenAll(
                from searcher in 
                    (from service in Host.Services
                     where service.GetType().IsSearcher()
                     select service).Cast<Searcher>()
                select searcher.SearchAsync(query));
            var results = resultGroup.SelectMany(group => group).ToList();

            if (Settings.MaximumResults != null) {
#if !DEBUG
                // Default behaviour
                results = new List<ResultInfo>(results.Take((int)Settings.MaximumResults.Value));
#else
                // Useful for debugging
                if (EnforceMaximumResults) {
                    results = new List<ResultInfo>(results.Take((int)Settings.MaximumResults.Value));
                }
#endif
            }

            // Invoke post processors in sync
            var processors = (from service in Host.Services
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

        static ServiceHost ConfigureServices(SearchContext context)
        {
            var searchers = context.Host.Services.CastOnlySubclassOf<Searcher>();
            var first = searchers.FirstOrDefault();
            uint? maximumResults = context.Settings.MaximumResults.HasValue
                ? context.Settings.MaximumResults / (uint?)searchers.Count()
                : null;
            var host = context.Host
                .Map<Service>(service => {
                    service.Context = context;
                    return service; })
                .Map<Searcher>(searcher => {
                    searcher.Policy = new RuntimePolicy(maximumResults);
                    return searcher; });
            if (first != null) {
                host = host.Map<Searcher>(searcher => {
                    var remainder = context.Settings.MaximumResults % (uint?)searchers.Count();
                    searcher.Policy = new RuntimePolicy(searcher.Policy.MaximumResults + remainder);
                    return searcher;
                    },
                    searcher => searcher.GetHashCode().Equals(first.GetHashCode()));
            }
            return host;
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