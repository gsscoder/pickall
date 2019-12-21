using System;
using System.Collections.Generic;
using System.Linq;
using SharpRhythm.Algorithms;

namespace PickAll.PostProcessors
{
    public class FuzzyMatchSettings
    {
        public string Text;

        public uint MinimumDistance;

        public uint MaximumDistance;
    }

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