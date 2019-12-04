using System;
using System.Collections.Generic;
using System.Linq;

namespace PickAll.PostProcessors
{

    public class FuzzyMatch : IPostProcessor
    {
        private readonly string _text;
        private readonly uint _minimumDistance;
        private readonly uint _maximumDistance;

        public FuzzyMatch(string text, uint minimumDistance, uint maximumDistance)
        {
            _text = text;
            _minimumDistance = minimumDistance;
            _maximumDistance = maximumDistance;
        }

        public FuzzyMatch(string text, uint maximumDistance) : this(text, 0, maximumDistance)
        {
        }

        public FuzzyMatch(string text) : this(text, 0, 0)
        {
        }

        public IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results)
        {
            return from computed in 
                       from result in results
                       select new {result = result,
                                   distance = LevenshteinDistance(_text, result.Description)}
                       where computed.distance >= _minimumDistance && computed.distance <= _maximumDistance
                   select computed.result;
        }

        // Based on https://www.dotnetperls.com/levenshtein.
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