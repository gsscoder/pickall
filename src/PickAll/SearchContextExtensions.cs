using System;
using System.Linq;
using System.Reflection;
using PickAll.Internal;

namespace PickAll
{
    /// <summary>
    /// A set of useful extensions for <see cref="SearchContext"/>.
    /// </summary>
    public static class SearchContextExtensions
    {
        /// <summary>
        /// Registers an instance of <see cref="Searcher"/> or <see cref="PostProcessor"/>
        /// without settings, using its type.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <param name="settings">The optional settings instance for the service.</param>
        /// <typeparam name="T">A type that inherits from <see cref="SearchContext"/>
        /// or <see cref="PostProcessor"/>.</typeparam>
        /// <returns>A <see cref="SearchContext"/> with the given service added.</returns>
        public static SearchContext With<T>(this SearchContext context, object settings = null)
        {
            if (!typeof(T).IsSearcher() && !typeof(T).IsPostProcessor()) {
                throw new NotSupportedException(
                    $"T must inherit from {nameof(Searcher)} or {nameof(PostProcessor)}");
            }

            var service = typeof(T).IsSearcher() 
                ? Activator.CreateInstance(typeof(T), settings, context.Settings.ToPolicy())
                : Activator.CreateInstance(typeof(T), settings);
            return new SearchContext(
                context.Services.Add(service).Cast<Service>(),
                new ContextSettings { MaximumResults = context.Settings.MaximumResults });
        }

        /// <summary>
        /// Registers an instance of <see cref="Searcher"/> or <see cref="PostProcessor"/>
        /// without settings, using its type name.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <param name="serviceName">Name of the service to register (case insensitive).</param>
        /// <param name="settings">The optional settings instance for the service.</param>
        /// <typeparam name="T">A type that inherits from <see cref="SearchContext"/>
        /// or <see cref="PostProcessor"/>.</typeparam>
        /// <returns>A <see cref="SearchContext"/> with the given service added.</returns>       
        public static SearchContext With(this SearchContext context, string serviceName,
                                         object settings = null)
        {
            if (serviceName == null) {
                throw new ArgumentNullException(nameof(serviceName), $"{nameof(serviceName)} cannot be null");
            }
            if (serviceName.Trim() == string.Empty) {
                throw new ArgumentException($"{nameof(serviceName)} cannot be empty or contain only space", nameof(serviceName));
            }

            var type = context.GetType().GetTypeInfo().Assembly.GetTypes().Where(
                @this => @this.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
                         .SingleOrDefault();
            if (type == null) {
                throw new NotSupportedException($"{serviceName} service not found");
            }
            if (!type.IsSearcher() && !type.IsPostProcessor()) {
                throw new NotSupportedException(
                    $"T must inherit from {nameof(Searcher)} or {nameof(PostProcessor)}");
            }

            var service = type.IsSearcher()
                ? Activator.CreateInstance(type, settings, context.Settings.ToPolicy())
                : Activator.CreateInstance(type, settings);
            return new SearchContext(
                context.Services.Add(service).Cast<Service>(),
                new ContextSettings { MaximumResults = context.Settings.MaximumResults });
        }

        /// <summary>
        /// Unregisters first instance of <see cref="Searcher"/> or <see cref="PostProcessor"/>,
        /// using its type.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <typeparam name="T">A type that inherits from <see cref="SearchContext"/>
        /// or <see cref="PostProcessor"/>.</typeparam>
        /// <returns>A <see cref="SearchContext"/> instance with the given service removed.</returns>
        public static SearchContext Without<T>(this SearchContext context)
        {
            if (!typeof(T).IsSearcher() && !typeof(T).IsPostProcessor()) {
                throw new NotSupportedException(
                    $"T must inherit from {nameof(Searcher)} or {nameof(PostProcessor)}");
            }

            return new SearchContext(
                context.Services.Remove(typeof(T)),
                new ContextSettings { MaximumResults = context.Settings.MaximumResults });
        }

        /// <summary>
        /// Unregisters first instance of <see cref="Searcher"/> or <see cref="PostProcessor"/>,
        /// using its type name.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <param name="serviceName">Name of the service to register (case insensitive).</param>
        /// or <see cref="PostProcessor"/>.</typeparam>
        /// <returns>A <see cref="SearchContext"/> instance with the given service removed.</returns>
        public static SearchContext Without(this SearchContext context, string serviceName)
        {
            if (serviceName == null) {
                throw new ArgumentNullException(nameof(serviceName), $"{nameof(serviceName)} cannot be null");
            }
            if (serviceName.Trim() == string.Empty) {
                throw new ArgumentException($"{nameof(serviceName)} cannot be empty or contain only space", nameof(serviceName));
            }

            var service = (from @this in context.Services
                           where @this.GetType().Name.Equals(
                               serviceName, StringComparison.OrdinalIgnoreCase)
                           select @this).FirstOrDefault();
            if (service == null) {
                throw new InvalidOperationException($"{serviceName} not registred as service");
            }
            return new SearchContext(
                context.Services.Remove(service.GetType()),
                new ContextSettings { MaximumResults = context.Settings.MaximumResults });
        }

        /// <summary>
        /// Unregisters all instances of types that inherits from <see cref="Searcher"/>
        /// or <see cref="PostProcessor"/>.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <typeparam name="T"><see cref="Searcher"/> or <see cref="PostProcessor"/> type.</typeparam>
        /// <returns>A <see cref="SearchContext"/> instance with all searcher
        /// or all postprocessor removed.</returns>
        public static SearchContext WithoutAll<T>(this SearchContext context)
        {
            if (typeof(T) != typeof(Searcher) && typeof(T) != typeof(PostProcessor))
            {
                throw new NotSupportedException(
                    $"T must be or inherit from {nameof(Searcher)} or {nameof(PostProcessor)}");
            }

            return new SearchContext(
                from service in context.Services
                where !service.GetType().IsSubclassOf(typeof(T))
                select service,
                new ContextSettings { MaximumResults = context.Settings.MaximumResults });
        }

        /// <summary>
        /// Configure a search context with a <see cref="ContextSettings"/> instance.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <param name="settings">The settings to configure in the search context.</param>
        /// <returns>A <see cref="SearchContext"/> instance with new configuration settings.</returns>
        public static SearchContext WithConfiguration(this SearchContext context,
            ContextSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings),
                $"{nameof(settings)} cannot be null");

            return new SearchContext(context.Services, settings);
        }

        /// <summary>
        /// Builds a new search context with same services of the current. 
        /// </summary>
        /// <param name="context">The search context to clone.</param>
        /// <returns>A cloned <see cref="SearchContext"/> instance.</returns>
        public static SearchContext Clone(this SearchContext context)
        {
            return new SearchContext(
                from service in context.Services
                select UnbindContext(service),
                new ContextSettings { MaximumResults = context.Settings.MaximumResults });
        }

        static Service UnbindContext(Service service)
        {
            var unbound = service;
            unbound.Context = null;
            return service;
        }
    }
}