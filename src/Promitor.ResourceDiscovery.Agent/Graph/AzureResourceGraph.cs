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

        public AzureResourceGraph(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<Resource>> QueryAsync()
        {
            var tenantId = "c8819874-9e56-4e3f-b1a8-1c0325138f27";
            var subscriptionId = "0f9d7fea-99e8-4768-8672-06a28514f77e";
            var appId = _configuration["DISCOVERY_APPID"];
            var appSecret = _configuration["DISCOVERY_APPSECRET"];
            var query = GraphQuery.ForResourceType("microsoft.servicebus/namespaces")
                .Project("subscriptionId", "resourceGroup", "type", "name", "id")
                .LimitTo(5)
                .Build();

            var credentials = await Authentication.GetServiceClientCredentialsAsync("https://management.core.windows.net", appId,appSecret, tenantId);
            ResourceGraphClient client = new ResourceGraphClient(credentials);

            var queryRequest = new QueryRequest(new List<string> {subscriptionId}, query);
            var response = await client.ResourcesAsync(queryRequest);
            
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
    }
}