using System;
using System.Collections.Generic;
using System.Linq;

namespace PickAll
{
    /// <summary>Settings for <c>FuzzyMatch</c> post processor.</summary>
    public struct FuzzyMatchSettings
    {
        int _minimumDistance;
        int _maximumDistance;

        /// <summary>String to compare against descriptions.</summary>
        public string Text { get; set; }

        /// <summary>Minimum distance of permutations.</summary>
        public int MinimumDistance
        {
            get { return _minimumDistance; }
            set
            { 
                Guard.AgainstNegative("MinimumDistance", value);
                _minimumDistance = value;
            }
        }

        /// <summary>Maximum distance of permutations.</summary>
        public int MaximumDistance
        {
            get { return _maximumDistance; }
            set
            { 
                Guard.AgainstNegative("MaximumDistance", value);
                _maximumDistance = value;
            }
        }
    }

    /// <summary>Compares a string against results descriptions.</summary>
    public class FuzzyMatch : PostProcessor
    {
        readonly FuzzyMatchSettings _settings;

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