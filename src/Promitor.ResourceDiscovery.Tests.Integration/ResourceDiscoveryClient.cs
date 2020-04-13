using System;
using System.Net.Http;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Promitor.ResourceDiscovery.Tests.Integration
{
    public class ResourceDiscoveryClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public ResourceDiscoveryClient(IConfiguration configuration, ILogger logger)
        {
            Guard.NotNull(configuration, nameof(configuration));
            Guard.NotNull(logger, nameof(logger));

            var baseUrl = configuration["Agent:BaseUrl"];
            logger.LogInformation("Base URL for discovery agent is '{Url}'", baseUrl);

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            _logger = logger;
        }

        public async Task<HttpResponseMessage> GetResourceCollectionsAsync()
        {
            return await _httpClient.GetAsync("/api/v1/resources/collections");
        }
    }
}