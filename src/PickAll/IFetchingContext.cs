using System.Threading.Tasks;

namespace PickAll
{
    public interface IFetchingContext
    {
        Task<IFetchedDocument> FetchAsync(string address);
    }
}