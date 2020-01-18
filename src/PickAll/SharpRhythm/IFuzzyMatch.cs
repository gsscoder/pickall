namespace SharpRhythm.Algorithms
{
    /// <summary>
    /// Represents a fuzzy match algorithm.
    /// </summary>
    interface IFuzzyMatch
    {
        uint Compare(string first, string second);
    }
}