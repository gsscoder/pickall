using System;

namespace PickAll
{
    public enum ServiceType
    {
        Searcher,
        PostProcessor
    }

    public sealed class SearchBeginEventArgs : EventArgs
    {
        public SearchBeginEventArgs(string query) => Query = query;

        public string Query { get; private set; }
    }

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