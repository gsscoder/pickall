using System;

namespace PickAll
{
    /// <summary>Settings for a search context.</summary>
    public struct ContextSettings
    {
        int? _maximumResults;
    
        /// <summary>Maximum results a search is allowed to return.</summary>
        public int? MaximumResults
        {
            get { return _maximumResults; }
            set
            {
                if (value.HasValue) Guard.AgainstNegative("MaximumResults", value.Value);
                _maximumResults = value;
            }
        }

        /// <summary>Timeout for each HTTP request performed.</summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>Enables events in search context and services.</summary>
        public bool EnableRaisingEvents { get; set; }

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
