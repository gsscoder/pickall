using System;

namespace PickAll
{
    /// <summary>
    /// Settings for a search context.
    /// </summary>
    public class ContextSettings
    {
        /// <summary>
        /// Maximum results a search is allowed to return.
        /// </summary>
        public uint? MaximumResults;

        /// <summary>
        /// Timeout for each HTTP request performed.
        /// </summary>
        public TimeSpan? Timeout;

        internal RuntimePolicy ToPolicy()
        {
            return new RuntimePolicy(MaximumResults);
        }
    }
}