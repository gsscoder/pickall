namespace PickAll
{
    public struct RuntimePolicy
    {
        internal RuntimePolicy(int? maximumResults)
        {
            if (maximumResults.HasValue) Guard.AgainstNegative(nameof(maximumResults), maximumResults.Value);

            MaximumResults = maximumResults;
        }

        public int? MaximumResults
        {
            get; private set;
        }
    }
}