/// <summary>
/// Models an<see cref="Searcher"/> result record.
/// </summary>
public class ResultInfo
{
#if DEBUG
    public ResultInfo()
    {
    }
#endif
    /// <summary>
    /// Initializes a new instance of <see cref="ResultInfo"/>.
    /// </summary>
    /// <param name="originator">The searcher which originated the result.</param>
    /// <param name="index">The result index.</param>
    /// <param name="url">The result URL.</param>
    /// <param name="description">The result description.</param>
    /// <param name="data">TAdditional data supplied by the service.</param>
    public ResultInfo(string originator, ushort index, string url, string description, object data)
    {
        Guard.AgainstNull(nameof(originator), originator);
        Guard.AgainstEmptyWhiteSpace(nameof(originator), originator);
        Guard.AgainstNull(nameof(url), url);
        Guard.AgainstEmptyWhiteSpace(nameof(url), url);
        Guard.AgainstNull(nameof(description), description);

        Originator = originator;
        Index = index;
        Url = url;
        Description = description;
        Data = data;
    }

    /// <summary>
    /// The searcher which originated the result.
    /// </summary>
    public string Originator
    {
        get;
#if DEBUG
        private set;
#else
        internal set;
#endif
    }

    /// <summary>
    /// The result index.
    /// </summary>
    public ushort Index
    {
        get;
#if !DEBUG
        private set;
#else
        internal set;
#endif
    }

    /// <summary>
    /// The result URL
    /// </summary>
    public string Url
    {
        get;
#if !DEBUG
        private set;
#else
        internal set;
#endif
    }

    /// <summary>
    /// The result description.
    /// </summary>
    public string Description 
    {
        get;
#if !DEBUG
        private set;
#else
        internal set;
#endif
    }
    /// <summary>
    /// Additional data supplied by the service. 
    /// </summary>
    public object Data
    {
        get;
#if !DEBUG
        private set;
#else
        internal set;
#endif
    }
} 