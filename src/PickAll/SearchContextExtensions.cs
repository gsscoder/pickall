using System;
using System.Collections.Generic;

namespace PickAll
{
    /// <summary>
    /// A set of useful extensions for <see cref="SearchContext">.
    /// </summary>
    public static partial class SearchContextExtensions
    {
        private static bool IsSearcher(Type type) => type.IsSubclassOf(typeof(Searcher)); 
        private static bool IsPostProcessor(Type type) => typeof(IPostProcessor).IsAssignableFrom(type);

        /// <summary>
        /// Registers an instance of <see cref="Searcher"> or <see cref="IPostProcessor">.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <param name="service">A service instance to register.</param>
        /// <returns>A <see cref="SearchContext"> with the given service added.</returns>
        public static SearchContext With(this SearchContext context, object service)
        {
            var type = service.GetType();
            if (!IsSearcher(type) && !IsPostProcessor(type)) {
                throw new NotSupportedException(
                    "T must inherit from Searcher or implements IPostProcessor");
            }
            return new SearchContext(context.Services.CopyWith(service));
        }

        /// <summary>
        /// Registers an instance of <see cref="Searcher"> or <see cref="IPostProcessor">
        /// with a parameterless constructor.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <typeparam name="T">A type that inherits from <see cref="SearchContext"> or
        /// implements <see cref="IPostProcessor">.</typeparam>
        /// <returns>A <see cref="SearchContext"> with the given service added.</returns>
        public static SearchContext With<T>(this SearchContext context)
        {
            var type = typeof(T);
            if (!IsSearcher(type) && !IsPostProcessor(type)) {
                throw new NotSupportedException(
                    "T must inherit from Searcher or implements IPostProcessor");
            }
            var service = Activator.CreateInstance<T>();
            return new SearchContext(context.Services.CopyWith(service));
        }

        /// <summary>
        /// Registers an instance of <see cref="Searcher"> or <see cref="IPostProcessor">
        /// using service name.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <param name="serviceName">Name of the service to add (case sensitive).</param>
        /// <param name="args">Optional arguments for service constructor.</param>
        /// <returns>A <see cref="SearchContext"> with the given service added.</returns>
        public static SearchContext With(this SearchContext context, string serviceName, params object[] args)
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
                    "T must inherit from Searcher or implements IPostProcessor");
            }

            var service = Activator.CreateInstance(type, args);
            return new SearchContext(context.Services.CopyWith(service));
        }

        /// <summary>
        /// Unregisters first instance of <see cref="Searcher"> or <see cref="IPostProcessor">
        /// using type.
        /// </summary>
        /// <param name="context">The search context to alter.</param>
        /// <typeparam name="T">A type that inherits from <see cref="SearchContext"> or
        /// implements <see cref="IPostProcessor">.</typeparam>
        /// <returns>A <see cref="SearchContext"> instance with the given service removed.</returns>
        public static SearchContext Without<T>(this SearchContext context)
        {
            var type = typeof(T);
            if (!IsSearcher(type) && !IsPostProcessor(type)) {
                throw new NotSupportedException(
                    "T must inherit from Searcher or implements IPostProcessor");
            }
            return new SearchContext(context.Services.CopyWithout<T>());
        }

        private static IEnumerable<object> CopyWith<T>(this IEnumerable<object> collection, T newElement)
        {
            foreach (var element in collection) {
                yield return element;
            }
            yield return newElement;
        }

        private static IEnumerable<object> CopyWithout<T>(this IEnumerable<object> collection)
        {
            var type = typeof(T);
            bool removed = false;
            foreach (var element in collection) {
                if (element.GetType() != type) {
                    yield return element;
                }
                else {
                    if (!removed) {
                        removed = true;
                    }
                    else {
                        yield return element;
                    }
                }
            }
        }
    }
}