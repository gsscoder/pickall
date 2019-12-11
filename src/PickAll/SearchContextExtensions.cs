using System;
using System.Linq;
using System.Reflection;

namespace PickAll
{
    /// <summary>
    /// A set of useful extensions for <see cref="SearchContext">.
    /// </summary>
    public static partial class SearchContextExtensions
    {
        /// <summary>
        /// Registers an instance of <see cref="Searcher"> or <see cref="PostProcessor">
        /// without settings, using its type.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <param name="settings">The optional settings instance for the service.</param>
        /// <typeparam name="T">A type that inherits from <see cref="SearchContext">
        /// or <see cref="PostProcessor">.</typeparam>
        /// <returns>A <see cref="SearchContext"> with the given service added.</returns>
        public static SearchContext With<T>(this SearchContext context, object settings = null)
        {
            var type = typeof(T);
            if (!SearchContext.IsSearcher(type) && !SearchContext.IsPostProcessor(type)) {
                throw new NotSupportedException(
                    "T must inherit from Searcher or PostProcessor");
            }
            var service = SearchContext.IsSearcher(type) ?
                Activator.CreateInstance(type, context.ActiveContext, settings) :
                Activator.CreateInstance(type, settings);
            return new SearchContext(context.Services.Concat(service));
        }

        /// <summary>
        /// Registers an instance of <see cref="Searcher"> or <see cref="PostProcessor">
        /// without settings, using its type name.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <param name="serviceName">Name of the service to register (case insensitive).</param>
        /// <param name="settings">The optional settings instance for the service.</param>
        /// <typeparam name="T">A type that inherits from <see cref="SearchContext">
        /// or <see cref="PostProcessor">.</typeparam>
        /// <returns>A <see cref="SearchContext"> with the given service added.</returns>       
        public static SearchContext With(this SearchContext context, string serviceName,
                                         object settings = null)
        {
            if (serviceName == null) {
                throw new ArgumentNullException($"{nameof(serviceName)} cannot be null");
            }
            if (serviceName.Trim() == string.Empty) {
                throw new ArgumentException($"{nameof(serviceName)} cannot be empty or contain only space");
            }
            var type = context.GetType().GetTypeInfo().Assembly.GetTypes().Where(
                @this => @this.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
                         .SingleOrDefault();
            if (type == null) {
                throw new NotSupportedException($"{serviceName} service not found");
            }
            if (!SearchContext.IsSearcher(type) && !SearchContext.IsPostProcessor(type)) {
                throw new NotSupportedException(
                    "T must inherit from Searcher or PostProcessor");
            }
            var service = SearchContext.IsSearcher(type) ?
                Activator.CreateInstance(type, context.ActiveContext, settings) :
                Activator.CreateInstance(type, settings);
            return new SearchContext(context.Services.Concat(service));
        }

        /// <summary>
        /// Unregisters first instance of <see cref="Searcher"> or <see cref="PostProcessor">,
        /// using its type.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <typeparam name="T">A type that inherits from <see cref="SearchContext">
        /// or <see cref="PostProcessor">.</typeparam>
        /// <returns>A <see cref="SearchContext"> instance with the given service removed.</returns>
        public static SearchContext Without<T>(this SearchContext context)
        {
            var type = typeof(T);
            if (!SearchContext.IsSearcher(type) && !SearchContext.IsPostProcessor(type)) {
                throw new NotSupportedException(
                    "T must inherit from Searcher or PostProcessor");
            }
            return new SearchContext(context.Services.Exclude<T>());
        }

        /// <summary>
        /// Unregisters first instance of <see cref="Searcher"> or <see cref="PostProcessor">,
        /// using its type name.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <param name="serviceName">Name of the service to register (case insensitive).</param>
        /// or <see cref="PostProcessor">.</typeparam>
        /// <returns>A <see cref="SearchContext"> instance with the given service removed.</returns>
        public static SearchContext Without(this SearchContext context, string serviceName)
        {
            if (serviceName == null) {
                throw new ArgumentNullException($"{nameof(serviceName)} cannot be null");
            }
            if (serviceName.Trim() == string.Empty) {
                throw new ArgumentException($"{nameof(serviceName)} cannot be empty or contain only space");
            }
            var service = (from @this in context.Services
                           where @this.GetType().Name.Equals(
                               serviceName, StringComparison.OrdinalIgnoreCase)
                           select @this).FirstOrDefault();
            if (service == null) {
                throw new InvalidOperationException($"{serviceName} not registred as service");
            }
            return new SearchContext(context.Services.Exclude(service.GetType()));
        }
    }
}