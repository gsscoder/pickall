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
        private readonly IBrowsingContext _context = BrowsingContext.New(
            Configuration.Default.WithDefaultLoader());
        private IEnumerable<object> _services =  new object[] {};
        private static bool IsSearcher<T>() => typeof(T).IsSubclassOf(typeof(Searcher)); 
        private static bool IsSearcher(Type type) => type.IsSubclassOf(typeof(Searcher)); 
        private static bool IsPostProcessor<T>() => typeof(IPostProcessor).IsAssignableFrom(typeof(T));
        private static bool IsPostProcessor(Type type) => typeof(IPostProcessor).IsAssignableFrom(type);

#if DEBUG
        public IEnumerable<object> Services
        {
            get { return _services; }
        }
#endif
        /// <summary>
        /// Registers an instance of <see cref="Searcher"> or <see cref="IPostProcessor">
        /// using type.
        /// </summary>
        /// <typeparam name="T">A type that inherits from <see cref="SearchContext"> or
        /// implements <see cref="IPostProcessor">.</typeparam>
        /// <returns>A <see cref="SearchContext"> with the given service added.</returns>
        public SearchContext With<T>()
        {
            if (IsSearcher<T>()) {
                _services = _services.CloneWith(CreateService<T>(_context));
            }
            else if (IsPostProcessor<T>()) {
                _services = _services.CloneWith(CreateService<T>());
            }
            else {
                throw new NotSupportedException(
                    "T must inherit from Searcher or implements IPostProcessor");
            }
            return this;
        }

        /// <summary>
        /// Registers an instance of <see cref="Searcher"> or <see cref="IPostProcessor">.
        /// </summary>
        /// <param name="service"></param>
        /// <typeparam name="T">A type that inherits from <see cref="SearchContext"> or
        /// implements <see cref="IPostProcessor">.</typeparam>
        /// <returns><A <see cref="SearchContext"> with the given service added./returns>
        /// <returns></returns>
        public SearchContext With<T>(T service)
        {
            if (service == null) {
                throw new ArgumentNullException($"{nameof(service)} cannot be null");
            }
            if (!IsSearcher<T>() && !IsPostProcessor<T>()) {
                throw new NotSupportedException(
                    $"${nameof(service)} must inherit from Searcher or implements IPostProcessor");
            }
            _services = _services.CloneWith(service);
            return this;
        }

        public SearchContext With(string serviceName)
        {
            if (serviceName == null) {
                throw new ArgumentNullException($"{nameof(serviceName)} cannot be null");
            }
            if (serviceName.Trim() == string.Empty) {
                throw new ArgumentException($"{nameof(serviceName)} cannot be empty or contain only space");
            }
            var type = Type.GetType($"PickAll.Searchers.{serviceName}", false);
            if (type == null) {
                type = Type.GetType($"PickAll.PostProcessors.{serviceName}", false);
                if (type == null) {
                    throw new NotSupportedException($"{serviceName} service not found");
                }
            }
            if (!IsSearcher(type) && !IsPostProcessor(type)) {
                throw new NotSupportedException(
                    $"${nameof(serviceName)} must inherit from Searcher or implements IPostProcessor");
            }
            _services = _services.CloneWith(CreateService(type, _context));
            return this;
        }

        /// <summary>
        /// Unregisters an instance of <see cref="Searcher"> or <see cref="IPostProcessor">.
        /// </summary>
        /// <typeparam name="T">A type that inherits from <see cref="SearchContext"> or
        /// implements <see cref="IPostProcessor">.</typeparam>
        /// <returns>A <see cref="SearchContext"> instance with the given service removed.</returns>
        public SearchContext Without<T>()
        {
            if (IsSearcher<T>()) {
                _services = _services.CloneWithout<T>();
            }
            else if (IsPostProcessor<T>()) {
                _services = _services.CloneWithout<T>();
            }
            else {
                throw new NotSupportedException(
                    "T must inherit from Searcher or implements IPostProcessor");
            }
            return this;
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
            foreach (var service in _services) {
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
            var @default = new SearchContext();
            @default._services = new object[]
                {
                    CreateService<Google>(@default._context),
                    CreateService<DuckDuckGo>(@default._context),
                    CreateService<Uniqueness>(),
                    CreateService<Order>()
                };
            return @default;
        }

        private static object CreateService<T>(IBrowsingContext context = null)
        {
            if (IsSearcher<T>()) {
                var service = (Searcher)Activator.CreateInstance(typeof(T));
                service.Context = context;
                return service;
            }
            else if (IsPostProcessor<T>()) {
                return Activator.CreateInstance(typeof(T));
            }
            throw new NotSupportedException(
                "T must inherit from Searcher or implements IPostProcessor");
        }

        private static object CreateService(Type type, IBrowsingContext context = null)
        {
            if (IsSearcher(type)) {
                var service = (Searcher)Activator.CreateInstance(type);
                service.Context = context;
                return service;
            }
            else if (IsPostProcessor(type)) {
                return Activator.CreateInstance(type);
            }
            throw new NotSupportedException(
                "T must inherit from Searcher or implements IPostProcessor");
        }
    }
}