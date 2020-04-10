using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceGraph;
using Microsoft.Azure.Management.ResourceGraph.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Promitor.ResourceDiscovery.Agent.Model;

namespace Promitor.ResourceDiscovery.Agent.Graph
{
    public class AzureResourceGraph
    {
        private readonly IConfiguration _configuration;
        private ResourceGraphClient _graphClient;

        public AzureResourceGraph(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<Resource>> QueryAsync(string resourceType)
        {
            var graphClient = await GetOrCreateClient();
            var subscriptionId = "0f9d7fea-99e8-4768-8672-06a28514f77e";
            var query = GraphQuery.ForResourceType(resourceType)
                .Project("subscriptionId", "resourceGroup", "type", "name", "id")
                .LimitTo(5)
                .Build();

            var queryRequest = new QueryRequest(new List<string> {subscriptionId}, query);
            var response = await graphClient.ResourcesAsync(queryRequest);
            
            var result = response.Data as JObject;
            var rows = result["rows"];
            var foundResources = new List<Resource>();
            foreach (var row in rows)
            {
                var resource = new Resource(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString());
                foundResources.Add(resource);
            }

            return foundResources;
        }

        private async Task<ResourceGraphClient> GetOrCreateClient()
        {
            if (_graphClient == null)
            {
                _graphClient = await CreateClientAsync();
            }

            return _graphClient;
        }

        private async Task<ResourceGraphClient> CreateClientAsync()
        {
            var tenantId = "c8819874-9e56-4e3f-b1a8-1c0325138f27";
            var appId = _configuration["DISCOVERY_APPID"];
            var appSecret = _configuration["DISCOVERY_APPSECRET"];

            var credentials = await Authentication.GetServiceClientCredentialsAsync("https://management.core.windows.net", appId, appSecret, tenantId);
            return new ResourceGraphClient(credentials);
        }
    }
}