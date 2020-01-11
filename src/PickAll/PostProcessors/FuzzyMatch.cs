using System;
using System.Collections.Generic;
using System.Linq;
using SharpRhythm.Algorithms;

namespace PickAll.PostProcessors
{
    /// <summary>
    /// Settings for <see cref="FuzzyMatch"/> post processor.
    /// </summary>
    public struct FuzzyMatchSettings
    {
        /// <summary>
        /// String to compare against descriptions.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Minimum distance of permutations. 
        /// </summary>
        public uint MinimumDistance { get; set; }

        /// <summary>
        /// Maximum distance of permutations.
        /// </summary>
        public uint MaximumDistance { get; set; }
    }

    /// <summary>
    /// Compares a string against results descriptions.
    /// </summary>
    public class FuzzyMatch : PostProcessor
    {
        private readonly FuzzyMatchSettings _settings;

        public FuzzyMatch(object settings) : base(settings)
        {
            if (!(Settings is FuzzyMatchSettings)) {
                throw new NotSupportedException($"{nameof(settings)} must be of FuzzyMatchSettings type");
            }
            _settings = (FuzzyMatchSettings)Settings;
        }

        public override IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
        {
            var fuzzyMatch = new LevenshteinFuzzyMatch(); 
            return
                from computed in 
                    from result in results
                    select new {result = result,
                                distance = fuzzyMatch.Compare(_settings.Text, result.Description)}
                    where computed.distance >= _settings.MinimumDistance &&
                          computed.distance <= _settings.MaximumDistance
                select computed.result;
        }
    }
}