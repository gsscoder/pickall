using System;

namespace PickAll
{
    /// <summary>Defines the service type.</summary>
    public enum ServiceType
    {
        /// <summary>Searcher service.</summary>
        Searcher,
        /// <summary>Post processor service.</summary>
        PostProcessor
    }

    /// <summary>Holds event data for the <c>SearchBegin</c> event.</summary>
    public sealed class SearchBeginEventArgs : EventArgs
    {
        public SearchBeginEventArgs(string query) => Query = query;

        public string Query { get; private set; }
    }

    /// <summary>Holds event data for the <c>ResultProcessed</c> event.</summary>
    public sealed class ResultHandledEventArgs : EventArgs
    {
        public ResultHandledEventArgs(ResultInfo result, ServiceType type)
        {
            Result = result;
            Type = type;
        }

        public ResultInfo Result { get; private set; }

        public ServiceType Type { get; private set; }
    } 
}