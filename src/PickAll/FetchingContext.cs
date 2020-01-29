using System.Net.Http;
using System.Threading.Tasks;

namespace PickAll
{
    public sealed class FetchingContext : IFetchingContext
    {
        private readonly HttpClient _client;

        #if DEBUG
        public FetchingContext(HttpClient httpClient) => _client = httpClient;
        #else
        internal FetchingContext(HttpClient httpClient) => _client = httpClient;
        #endif

        public async Task<IFetchedDocument> FetchAsync(string address)
        {
            Guard.AgainstNull(nameof(address), address);
            Guard.AgainstEmptyWhiteSpace(nameof(address), address);

            try {
                var response = await _client.GetAsync(address);
                if (!response.IsSuccessStatusCode) {
                    return FetchedDocument.Empty;
                }
                return new FetchedDocument(await response.Content.ReadAsByteArrayAsync());
            }
            catch (HttpRequestException) {
                return FetchedDocument.Empty;
            }
        }
    }
}