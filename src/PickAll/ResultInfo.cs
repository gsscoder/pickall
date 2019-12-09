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
        /// <param name="data">TAdditional data supplied by the service.</param>
        public ResultInfo(string originator, ushort index, string url, string description, object data)
        {
            Originator = originator;
            Index = index;
            Url = url;
            Description = description;
            Data = data;
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

        /// <summary>
        /// Additional data supplied by the service. 
        /// </summary>
        public object Data { get; private set; }
    } 
}