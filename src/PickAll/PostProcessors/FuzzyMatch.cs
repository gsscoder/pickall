using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return
                from computed in 
                    from result in results
                    select new {result = result,
                                distance = LevenshteinDistance(_settings.Text, result.Description)}
                    where computed.distance >= _settings.MinimumDistance &&
                          computed.distance <= _settings.MaximumDistance
                select computed.result;
        }

        /// <summary>
        /// Based on https://www.dotnetperls.com/levenshtein.
        /// </summary>
        private static uint LevenshteinDistance(string firstString, string secondString)
        {
            uint n = (uint)firstString.Length;
            uint m = (uint)secondString.Length;
            uint[,] d = new uint[n + 1, m + 1];

            // Step 1
            if (n == 0) {
                return m;
            }
            if (m == 0) {
                return n;
            }
            // Step 2
            for (uint i = 0; i <= n; d[i, 0] = i++) {
            }
            for (uint j = 0; j <= m; d[0, j] = j++) {
            }
            // Step 3
            for (uint i = 1; i <= n; i++) {
                //Step 4
                for (uint j = 1; j <= m; j++) {
                    // Step 5
                    uint cost = (secondString[(int)j - 1] == firstString[(int)i - 1]) ? 0 : 1u;
                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}