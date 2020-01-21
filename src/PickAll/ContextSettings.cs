using System;

namespace PickAll
{
    /// <summary>
    /// Settings for a search context.
    /// </summary>
    public struct ContextSettings
    {
        /// <summary>
        /// Maximum results a search is allowed to return.
        /// </summary>
        public uint? MaximumResults;

        /// <summary>
        /// Timeout for each HTTP request performed.
        /// </summary>
        public TimeSpan? Timeout;

        /// <summary>
        /// Enables events in search context and services.
        /// </summary>
        public bool EnableRaisingEvents;

        internal RuntimePolicy ToPolicy()
        {
            return new RuntimePolicy(MaximumResults);
        }

        internal ContextSettings Clone()
        {
            return new ContextSettings
                {
                    MaximumResults = MaximumResults,
                    Timeout = Timeout,
                    EnableRaisingEvents = EnableRaisingEvents
                };
        }
    }
}