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
            where T : Service
        {
            Guard.AgainstNull(nameof(context), context);

            var service = (T)Activator.CreateInstance(typeof(T), settings);
            return new SearchContext(
                context.Services.Add(service),
                context.Settings.Clone());
        }

        /// <summary>Registers an instance of <c>Searcher</c> or <c>PostProcessor</c> without settings,
        /// using its type name.</summary>
        public static SearchContext With(this SearchContext context, string serviceName,
                                         object settings = null)
        {
            Guard.AgainstNull(nameof(context), context);
            Guard.AgainstNull(nameof(serviceName), serviceName);
            Guard.AgainstEmptyWhiteSpace(nameof(serviceName), serviceName);

            var type = context.GetType().GetTypeInfo().Assembly.GetTypes().Where(
                @this => @this.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
                            .SingleOrDefault();
            if (type == null) {
                throw new NotSupportedException($"{serviceName} service not found");
            }

            var service = Activator.CreateInstance(type, settings);
            Guard.AgainstSubclassExcept<Service>(nameof(serviceName), service);
            return new SearchContext(
                context.Services.Add(service),
                context.Settings.Clone());
        }

        /// <summary>Unregisters first instance of <c>Searcher</c> or <c>PostProcessor</c>, using its
        /// type.</summary>
        public static SearchContext Without<T>(this SearchContext context)
            where T : Service
        {
            Guard.AgainstNull(nameof(context), context);

            return new SearchContext(
                context.Services.Remove<T>(),
                context.Settings.Clone());
        }

        /// <summary>Unregisters first instance of <c>Searcher</c> or <c>PostProcessor</c>, using its
        /// type name.</summary>
        public static SearchContext Without(this SearchContext context, string serviceName)
        {
            Guard.AgainstNull(nameof(context), context);
            Guard.AgainstNull(nameof(serviceName), serviceName);
            Guard.AgainstEmptyWhiteSpace(nameof(serviceName), serviceName);

            var service = (from @this in context.Services
                           where @this.GetType().Name.Equals(
                                 serviceName, StringComparison.OrdinalIgnoreCase)
                           select @this).FirstOrDefault();
            if (service == null) {
                throw new InvalidOperationException($"{serviceName} not registred as service");
            }
            return new SearchContext(
                context.Services.Remove(service.GetType()),
                context.Settings.Clone());
        }

        /// <summary>Unregisters all instances of types that inherits from <c>Searcher</c>
        /// or <c>PostProcessor</c>.</summary>
        public static SearchContext WithoutAll<T>(this SearchContext context)
            where T : Service
        {
            Guard.AgainstNull(nameof(context), context);

            return new SearchContext(
                context.Services.RemoveAll<T>(),
                context.Settings.Clone());
        }

        /// <summary>Configures a search context with a <c>ContextSettings</c> instance. If <c>merge</c>
        /// is <c>true</c> the settings instance is merged to the actul one.</summary>
        public static SearchContext WithConfiguration(this SearchContext context,
                                                      ContextSettings settings, bool merge = false)
        {
            Guard.AgainstNull(nameof(context), context);

            if (merge) {
                var merged = context.Settings;
                if (settings.MaximumResults != default(int?)) merged.MaximumResults = settings.MaximumResults;
                if (settings.Timeout != default(TimeSpan)) merged.Timeout = settings.Timeout;
                if (settings.EnableRaisingEvents != default(bool)) merged.EnableRaisingEvents = settings.EnableRaisingEvents;
                return new SearchContext(context.Services, merged);
            }
            return new SearchContext(context.Services, settings);
        }

        /// <summary>Configures a search context able to raise events.</summary>
        public static SearchContext WithEvents(this SearchContext context)
        {
            Guard.AgainstNull(nameof(context), context);

            return context.WithConfiguration(
                new ContextSettings { EnableRaisingEvents = true }, merge: true);
        }

        /// <summary>Builds a new search context with same services of the current.</summary>
        public static SearchContext Clone(this SearchContext context)
        {
            Guard.AgainstNull(nameof(context), context);

            return new SearchContext(
                context.Services.Map<Service>(service =>
                    {
                        service.Context = null;
                        return service;
                    }),
                new ContextSettings { MaximumResults = context.Settings.MaximumResults });
        }
    }
}
