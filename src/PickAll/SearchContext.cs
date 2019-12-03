using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp;

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
        private object WithContext<T>() => Activator.CreateInstance(typeof(T), _context);
        private static T WithoutContext<T>() => Activator.CreateInstance<T>();

        /// <summary>
        /// Registers an instance of <see cref="Searcher"> or <see cref="IPostProcessor">.
        /// </summary>
        /// <typeparam name="T">A type that inherits from <see cref="SearchContext"> or
        /// implements <see cref="IPostProcessor">.</typeparam>
        /// <returns>A <see cref="SearchContext"> with the given service added.</returns>
        public SearchContext With<T>()
        {
            if (typeof(T).IsSubclassOf(typeof(Searcher))) {
                _services = _services.CloneWith(WithContext<T>());
            }
            else if (typeof(IPostProcessor).IsAssignableFrom(typeof(T))) {
                _services = _services.CloneWith(WithoutContext<T>());
            }
            else {
                throw new NotSupportedException(
                    "T must inherit from Searcher or implements IPostProcessor");
            }
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
            if (typeof(T).IsSubclassOf(typeof(Searcher))) {
                _services = _services.CloneWithout<T>();
            }
            else if (typeof(IPostProcessor).IsAssignableFrom(typeof(T))) {
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
        public async Task<IEnumerable<ResultInfo>> Search(string query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query),
                $"{nameof(query)} cannot be null");
            if (query.Trim() == string.Empty) throw new ArgumentException(nameof(query),
                $"{nameof(query)} cannot be empty or contains only white spaces");

            var searches = new List<ResultInfo>();
            foreach (var searcher in _services.CastOnlySubclassOf<Searcher>()) {
                searches.AddRange(await searcher.Search(query));
            }

            var processed = new List<ResultInfo>();
            processed.AddRange(searches);
            foreach (var postProcessor in _services.CastImplements<IPostProcessor>()) {
                 var current = postProcessor.Process(processed);
                 processed = new List<ResultInfo>();
                 processed.AddRange(current);
            }

            return processed;
        }

        /// <summary>
        /// Builds a <see cref="SearchContext"> instance, registering default services.
        /// </summary>
        /// <returns>A <see cref="SearchContext"> instance.</returns>
        public static SearchContext Default()
        {
            var context = new SearchContext();
            context._services = new object[]
                {
                    context.WithContext<GoogleSearcher>(),
                    context.WithContext<DuckDuckGoSearcher>(),
                    WithoutContext<UniquenessPostProcessor>(),
                    WithoutContext<OrderPostProcessor>()
                };
            return context;
        }
    }
}