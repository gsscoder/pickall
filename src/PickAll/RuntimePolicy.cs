namespace PickAll
{
    public struct RuntimeInfo
    {
        internal RuntimeInfo(string query, int? maximumResults)
        {
            Query = query;
            MaximumResults = maximumResults;
        }

        public string Query { get; set; }

        public int? MaximumResults { get; private set; }
    }
}