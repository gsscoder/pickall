namespace PickAll
{
    /// <summary>
    /// Models an<see cref="Searcher"/> result record.
    /// </summary>
    public class ResultInfo
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ResultInfo">.
        /// </summary>
        /// <param name="originator">The searcher which originated the result.</param>
        /// <param name="index">The result index.</param>
        /// <param name="url">The result URL.</param>
        /// <param name="description">The result description.</param>
        public ResultInfo(string originator, ushort index, string url, string description)
        {
            Originator = originator;
            Index = index;
            Url = url;
            Description = description;
        }

        /// <summary>
        /// The searcher which originated the result.
        /// </summary>
        public string Originator { get; private set;}

        /// <summary>
        /// The result index.
        /// </summary>
        public ushort Index { get; private set; }

        /// <summary>
        /// The result URL
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// The result description.
        /// </summary>
        public string Description { get; private set; }
    } 
}