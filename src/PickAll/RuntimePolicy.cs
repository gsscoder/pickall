namespace PickAll
{
    public struct RuntimePolicy
    {
        internal RuntimePolicy(uint? maximumResults)
        {
            MaximumResults = maximumResults;
        }

        public uint? MaximumResults
        {
            get; private set;
        }
    }
}