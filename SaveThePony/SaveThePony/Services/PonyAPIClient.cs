using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SaveThePony.Services
{
    public class PonyAPIClient : IPonyAPIClient
    {
        public PonyAPIClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        // Gets the list of services on github.
        public async Task<HttpResponseMessage> GetMaze(Guid mazeId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/{mazeId}");

            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
