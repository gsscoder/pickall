using System.Collections.Generic;

/// <summary>
/// Represents a post processor service managed by <see cref="SearchContext"/>.
/// </summary>
public abstract class PostProcessor : Service
{
    public PostProcessor(object settings)
    {
        Settings = settings;
    }

    public abstract IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results);
}