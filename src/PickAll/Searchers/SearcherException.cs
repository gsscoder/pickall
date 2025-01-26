namespace PickAll.Searchers;

public class SearcherException : Exception
{
    public SearcherException(string message): base(message)
    {
    }

    public SearcherException(string message,Exception innerException) : base(message, innerException)
    {
    }
}
