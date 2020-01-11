namespace PickAll
{
    public struct RuntimePolicy
    {
        public RuntimePolicy(uint? maximumResults)
        {
            MaximumResults = maximumResults;
        }

        public uint? MaximumResults
        {
            get; private set;
        }
    }
}