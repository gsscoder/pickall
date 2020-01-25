using System;
using System.Linq;
using System.Reflection;

namespace PickAll
{
    /// <summary>A set of useful extensions for <c>SearchContext</c>.</summary>
    public static class SearchContextExtensions
    {
        /// <summary>Registers an instance of <c>Searcher</c> or <c>PostProcessor</c> without settings,
        /// using its type.</summary>
        public static SearchContext With<T>(this SearchContext context, object settings = null)
        {
            var service = Activator.CreateInstance(typeof(T), settings);
            return new SearchContext(
                context.Host.Clone().Add(service),
                context.Settings.Clone());
        }

        /// <summary>Registers an instance of <c>Searcher</c> or <c>PostProcessor</c> without settings,
        /// using its type name.</summary>
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

            var service = Activator.CreateInstance(type, settings);
            return new SearchContext(
                context.Host.Clone().Add(service),
                context.Settings.Clone());
        }

        /// <summary>Unregisters first instance of <c>Searcher</c> or <c>PostProcessor</c>, using its
        /// type.</summary>
        public static SearchContext Without<T>(this SearchContext context)
        {
            return new SearchContext(
                context.Host.Clone().Remove<T>(),
                context.Settings.Clone());
        }

        /// <summary>Unregisters first instance of <c>Searcher</c> or <c>PostProcessor</c>, using its
        /// type name.</summary>
        public static SearchContext Without(this SearchContext context, string serviceName)
        {
            if (serviceName == null) {
                throw new ArgumentNullException(nameof(serviceName), $"{nameof(serviceName)} cannot be null");
            }
            if (serviceName.Trim() == string.Empty) {
                throw new ArgumentException($"{nameof(serviceName)} cannot be empty or contain only space", nameof(serviceName));
            }

            var service = (from @this in context.Host.Services
                           where @this.GetType().Name.Equals(
                                 serviceName, StringComparison.OrdinalIgnoreCase)
                           select @this).FirstOrDefault();
            if (service == null) {
                throw new InvalidOperationException($"{serviceName} not registred as service");
            }
            return new SearchContext(
                context.Host.Clone().Remove(service.GetType()),
                context.Settings.Clone());
        }

        /// <summary>Unregisters all instances of types that inherits from <c>Searcher</c>
        /// or <c>PostProcessor</c>.</summary>
        public static SearchContext WithoutAll<T>(this SearchContext context)
        {
            return new SearchContext(
                context.Host.Clone().RemoveAll<T>(),
                context.Settings.Clone());
        }

        /// <summary>Configures a search context with a <c>ContextSettings</c> instance.</summary>
        public static SearchContext WithConfiguration(this SearchContext context,
            ContextSettings settings)
        {
            return new SearchContext(context.Host.Clone(), settings);
        }

        /// <summary>Builds a new search context with same services of the current.</summary>
        public static SearchContext Clone(this SearchContext context)
        {
            return new SearchContext(
                context.Host.Map(service => UnbindContext(service)),
                new ContextSettings { MaximumResults = context.Settings.MaximumResults });
        }

        static object UnbindContext(object service)
        {
            var unbound = (Service)service;
            unbound.Context = null;
            return service;
        }
    }
}