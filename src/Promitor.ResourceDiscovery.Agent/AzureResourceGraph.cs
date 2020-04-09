using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceGraph;
using Microsoft.Azure.Management.ResourceGraph.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Newtonsoft.Json.Linq;

namespace Promitor.ResourceDiscovery.Agent
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
            var query = "Resources | project subscriptionId, resourceGroup, type, name, id | limit 5";

            var credentials = await AuthenticationHelper.GetServiceClientCredentialsAsync("https://management.core.windows.net", appId,appSecret, tenantId);
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

    public class Resource
    {
        public Resource(string subscriptionId, string resourceGroup, string type, string name)
        {
            SubscriptionId = subscriptionId;
            ResourceGroup = resourceGroup;
            Name = name;
            Type = type;
        }

        public string SubscriptionId { get; }
        public string ResourceGroup { get; }
        public string Name { get; }
        public string Type { get;  }
    }

    public static class AuthenticationHelper
    {
        public static async Task<ServiceClientCredentials> GetServiceClientCredentialsAsync(string resource, string clientId, string clientSecret, string tenantId)
        {
            AuthenticationContext authContext = new AuthenticationContext($"https://login.microsoftonline.com/{tenantId}");

            AuthenticationResult authResult = await authContext.AcquireTokenAsync(resource, new ClientCredential(clientId, clientSecret));

            ServiceClientCredentials serviceClientCreds = new TokenCredentials(authResult.AccessToken);

            return serviceClientCreds;
        }
    }
}