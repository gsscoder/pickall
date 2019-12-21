namespace PickAll
{
    /// <summary>
    /// Models the <see cref="SearchContext"> state for services.
    /// </summary>
    public sealed class ContextState
    {
        internal ContextState(string query)
        {
            Query = query;
        }

        /// <summary>
        /// Query used in a given search context.
        /// </summary>
        public string Query
        {
            get; private set;
        }
    }
}