using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Promitor.ResourceDiscovery.Tests.Integration
{
    public class ResourceDiscoveryClient
    {
        private readonly HttpClient _httpClient;

        public ResourceDiscoveryClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:7777") // TODO: Read from config
            };
        }

        public async Task<HttpResponseMessage> GetResourceCollectionsAsync()
        {
            return await _httpClient.GetAsync("/api/v1/resources/collections");
        }
    }
}