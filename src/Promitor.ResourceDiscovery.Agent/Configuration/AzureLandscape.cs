using System.Collections.Generic;

namespace Promitor.ResourceDiscovery.Agent.Configuration
{
    public class AzureLandscape
    {
        public string TenantId { get; set; }
        public List<string> Subscriptions { get; set; }
    }
}
