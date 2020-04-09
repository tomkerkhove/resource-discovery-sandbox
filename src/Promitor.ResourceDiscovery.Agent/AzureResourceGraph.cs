using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Promitor.ResourceDiscovery.Agent
{
    public class AzureResourceGraph
    {
        private readonly IConfiguration _configuration;

        public AzureResourceGraph(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<List<Object>> QueryAsync()
        {
            var appId = _configuration["DISCOVERY_APPID"];
            var appSecret = _configuration["DISCOVERY_APPSECRET"];

            var foundResources = new List<object>
            {
                new {subscriptionId="ABC"}
            };

            return Task.FromResult(foundResources);
        }
    }
}