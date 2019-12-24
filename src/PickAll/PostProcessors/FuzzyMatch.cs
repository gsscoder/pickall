using System;
using System.Collections.Generic;
using System.Linq;
using SharpRhythm.Algorithms;

namespace PickAll.PostProcessors
{
    /// <summary>
    /// Settings for <see cref="FuzzyMatch"/> post processor.
    /// </summary>
    public class FuzzyMatchSettings
    {
        /// <summary>
        /// String to compare against descriptions.
        /// </summary>
        public string Text;

        /// <summary>
        /// Minimum distance of permutations. 
        /// </summary>
        public uint MinimumDistance;

        /// <summary>
        /// Maximum distance of permutations.
        /// </summary>
        public uint MaximumDistance;
    }

    /// <summary>
    /// Post processor to compare a string against results descriptions
    /// </summary>
    public class FuzzyMatch : PostProcessor
    {
        private readonly FuzzyMatchSettings _settings;

        public FuzzyMatch(object settings) : base(settings)
        {
            _settings = Settings as FuzzyMatchSettings;
            if (_settings == null) {
                throw new NotSupportedException($"{nameof(settings)} must be of FuzzyMatchSettings type");
            }
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