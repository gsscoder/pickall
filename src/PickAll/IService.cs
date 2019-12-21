namespace PickAll
{
    /// <summary>
    /// Represents a service managed by <see cref="SearchContext"/>.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Current state of search context.
        /// </summary>
        ContextState State { get; set; }
    }
}