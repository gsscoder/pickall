using System;
using System.Collections.Generic;

namespace PickAll
{
    /// <summary>Represents a post processor service managed by <c>SearchContext</c>.</summary>
    public abstract class PostProcessor : Service
    {
        public PostProcessor(object settings)
        {
            Settings = settings;
        }

        public virtual bool PublishEvents => false;

        public abstract IEnumerable<ResultInfo> Process(IEnumerable<ResultInfo> results);
    }
}
