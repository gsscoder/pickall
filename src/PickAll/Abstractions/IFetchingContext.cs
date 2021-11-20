using System.Threading.Tasks;

namespace PickAll
{
    /// <summary>Represents a context in which the a document without HTML DOM is fetched.</summary>
    public interface IFetchingContext
    {
        Task<IFetchedDocument> FetchAsync(string address);
    }
}
