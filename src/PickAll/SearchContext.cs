using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using AngleSharp;
using AngleSharp.Io.Network;

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

        public SearchContext(int maximumResults)
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

        public event EventHandler<SearchBeginEventArgs> SearchBegin;
        public event EventHandler SearchEnd;
        public event EventHandler ServiceLoad;
        public event EventHandler<ResultHandledEventArgs> ResultCreated;
        public event EventHandler<ResultHandledEventArgs> ResultProcessed;
        public IBrowsingContext Browsing { get { return _activeContext.Value; } }
        public string Query { get; private set; }
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
            Guard.AgainstNull(nameof(query), query);
            Guard.AgainstEmptyWhiteSpace(nameof(query), query);

            Query = query;

            EventHelper.RaiseEvent(this, SearchBegin,
                () => new SearchBeginEventArgs(Query), Settings.EnableRaisingEvents);

            // Bind context and partition maximum results
            Host = Configure(this);

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
                var publish = Settings.EnableRaisingEvents && processor.PublishEvents;
                var current = processor.Process(results);
                results = new List<ResultInfo>();
                foreach (var result in current) {
                    EventHelper.RaiseEvent(this, ResultProcessed,
                        () => new ResultHandledEventArgs(result, ServiceType.PostProcessor), publish);
                    results.Add(result);
                }
            }

            EventHelper.RaiseEvent(this, SearchEnd, EventArgs.Empty, Settings.EnableRaisingEvents);

            return results;
        }

        /// <summary>
        /// Builds a <see cref="SearchContext"/> instance with default services.
        /// </summary>
        public static SearchContext Default
        {
            get { return _default.Value; }
        }

        static ServiceHost Configure(SearchContext context)
        {
            var searchers = context.Host.Services.OfType<Searcher>();
            var first = searchers.FirstOrDefault();
            var maximumResults = context.Settings.MaximumResults.HasValue
                ? context.Settings.MaximumResults / searchers.Count()
                : null;
            var host = context.Host
                .Configure<Service>(service => ConfigureService(service))
                .Configure<Searcher>(searcher => ConfigureSearcher(searcher));
            if (first != null) {
                // first service maybe burdened of handling extra results 
                host = host.Configure<Searcher>(searcher =>
                    searcher.Policy = new RuntimePolicy(
                        searcher.Policy.MaximumResults +
                        context.Settings.MaximumResults % searchers.Count()),
                    searcher => searcher.GetHashCode().Equals(first.GetHashCode()));
            }
            return host;

            Service ConfigureService(Service service)
            {
                service.Load += context.ServiceLoad;
                service.Context = context;
                return service;
            }
            Searcher ConfigureSearcher(Searcher searcher)
            {
                searcher.ResultCreated += context.ResultCreated;
                searcher.Policy = new RuntimePolicy(maximumResults);
                return searcher;
            }
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