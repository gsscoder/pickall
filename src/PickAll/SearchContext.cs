using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using AngleSharp;
using AngleSharp.Io.Network;

namespace PickAll
{
    /// <summary>Manages <c>Searcher</c> and <c>PostProcessor</c> instances to gather
    /// and elaborate results.</summary>
    public sealed class SearchContext
    {   
        Lazy<IBrowsingContext> _browsing;
        Lazy<IFetchingContext> _fetching;
        static readonly Lazy<SearchContext> _default = new Lazy<SearchContext>(
            () => new SearchContext(
                typeof(Google),
                typeof(DuckDuckGo),
                typeof(Uniqueness),
                typeof(Order)));

        internal SearchContext(IEnumerable<object> services, ContextSettings settings)
        {
            Services = services;
            Settings = settings;
            _browsing = new Lazy<IBrowsingContext>(
                () => BuildBrowsingContext(settings.Timeout, () => BuildHttpClient(settings.Timeout)));
            _fetching = new Lazy<IFetchingContext>(
                () => new FetchingContext(BuildHttpClient(settings.Timeout, new HttpClient())));
        #if DEBUG
            EnforceMaximumResults = true;
        #endif
        }

        public SearchContext(ContextSettings settings)
            : this(Enumerable.Empty<object>(), settings) { }

        public SearchContext()
            : this(Enumerable.Empty<object>(), new ContextSettings()) { }

        public SearchContext(int maximumResults)
            : this(Enumerable.Empty<object>(), new ContextSettings { MaximumResults = maximumResults }) { }

        public SearchContext(TimeSpan timeout)
            : this(Enumerable.Empty<object>(), new ContextSettings { Timeout = timeout }) { }

        /// <summary>Builds a new search context with a given types.</summary>
        public SearchContext(params Type[] services) : this()
        {
            Guard.AgainstSubclassExcept<Service>(nameof(services), services);

            Services = Enumerable.Empty<object>();
            foreach (var type in services) {
                var instance = Activator.CreateInstance(type, new object[] { null });
                Services = Services.Add(instance);
            }
        }

        public event EventHandler<SearchBeginEventArgs> SearchBegin;
        public event EventHandler SearchEnd;
        public event EventHandler ServiceLoad;
        public event EventHandler<ResultHandledEventArgs> ResultCreated;
        public event EventHandler<ResultHandledEventArgs> ResultProcessed;
#pragma warning disable CS3003
        public IBrowsingContext Browsing { get { return _browsing.Value; } }
#pragma warning restore CS3003
        public IFetchingContext Fetching { get { return _fetching.Value; } }
        public string Query { get; private set; }
    #if !DEBUG
        internal IEnumerable<object> Services { get; private set; }
        internal ContextSettings Settings { get; private set; }
    #else
        public IEnumerable<object> Services { get; private set; }
        public ContextSettings Settings { get; private set; }
        public bool EnforceMaximumResults { get; set; } // Debug only
    #endif

        /// <summary>Executes a search using the given <c>query</c>, invoking all <c>Searcher</c>
        /// services asynchronously and then <c>PostProcessor</c> services in chain. Returns a
        /// sequence of <c>ResultInfo</c>.</summary>
        public async Task<IEnumerable<ResultInfo>> SearchAsync(string query)
        {
            Guard.AgainstNull(nameof(query), query);
            Guard.AgainstEmptyWhiteSpace(nameof(query), query);

            Query = query;

            EventHelper.RaiseEvent(this, SearchBegin,
                () => new SearchBeginEventArgs(Query), Settings.EnableRaisingEvents);

            // Bind context and partition maximum results
            Services = Configure(this);

            // Invoke searchers in parallel
            var resultGroup = await Task.WhenAll(
                from searcher in Services.OfType<Searcher>()
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
            foreach (var processor in Services.OfType<PostProcessor>()) {
                var publish = Settings.EnableRaisingEvents && processor.PublishEvents;
                var current = processor.Process(results);
                results = new List<ResultInfo>();
                foreach (var result in current) {
                    EventHelper.RaiseEvent(processor, ResultProcessed,
                        () => new ResultHandledEventArgs(result, ServiceType.PostProcessor), publish);
                    results.Add(result);
                }
            }

            EventHelper.RaiseEvent(this, SearchEnd, EventArgs.Empty, Settings.EnableRaisingEvents);

            return results;
        }

        /// <summary>Builds a <c>SearchContext</c> instance with default services.</summary>
        public static SearchContext Default
        {
            get { return _default.Value; }
        }

        static IEnumerable<object> Configure(SearchContext context)
        {
            var searchers = context.Services.OfType<Searcher>();
            var first = searchers.FirstOrDefault();
            var maximumResults = context.Settings.MaximumResults.HasValue
                ? context.Settings.MaximumResults / searchers.Count()
                : null;
            var services = context.Services
                .Map<Service>(service =>
                    {
                        service.Load += context.ServiceLoad;
                        service.Context = context;
                        return service;
                    })
                .Map<Searcher>(searcher =>
                    {
                        searcher.ResultCreated += context.ResultCreated;
                        searcher.Policy = new RuntimePolicy(maximumResults);
                        return searcher;
                    });
            if (first != null) {
                // first service maybe burdened of handling extra results 
                services = services.Map<Searcher>(searcher =>
                    {
                        searcher.Policy = new RuntimePolicy(
                            searcher.Policy.MaximumResults +
                            context.Settings.MaximumResults % searchers.Count());
                        return searcher;
                    },
                    searcher => searcher.GetHashCode().Equals(first.GetHashCode()));
            }
            return services;
        }

        static HttpClient BuildHttpClient(TimeSpan? timeout, HttpClient defaultClient = null)
        {
            if (timeout.HasValue) {
                var client = new HttpClient();
                client.Timeout = timeout.Value;
                return client;
            }
            return defaultClient;
        }

        static IBrowsingContext BuildBrowsingContext(
            TimeSpan? timeout, Func<HttpClient> client)
        {
            if (timeout.HasValue) {
                var requester = new HttpClientRequester(client());
                return BrowsingContext.New(
                    Configuration.Default
                        .WithRequester(requester)
                        .WithDefaultLoader());
            }
            return BrowsingContext.New(Configuration.Default.WithDefaultLoader());
        }
    }
}