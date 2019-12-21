/// Based on https://www.dotnetperls.com/levenshtein.
using System;

namespace SharpRhythm.Algorithms
{
    /// <summary>
    /// Levenshtein distance fuzzy match algorithm.
    /// </summary>
    class LevenshteinFuzzyMatch : IFuzzyMatch
    {
        public uint Compare(string first, string second)
        {
            uint n = (uint)first.Length;
            uint m = (uint)second.Length;
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
                    uint cost = (second[(int)j - 1] == first[(int)i - 1]) ? 0 : 1u;
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