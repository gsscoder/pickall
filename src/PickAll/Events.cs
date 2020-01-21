using System;

namespace PickAll
{
    public sealed class SearchBeginEventArgs : EventArgs
    {
        public SearchBeginEventArgs(string query)
        {
            Query = query;
        }

        public string Query { get; private set; }
    }

    public sealed class ResultHandledEventArgs : EventArgs
    {
        public ResultHandledEventArgs(ResultInfo result)
        {
            Result = result;
        }

        public ResultInfo Result { get; private set; }
    } 
}