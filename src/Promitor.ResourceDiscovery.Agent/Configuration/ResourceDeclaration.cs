using System.Collections.Generic;

namespace Promitor.ResourceDiscovery.Agent.Configuration
{
    public class ResourceDeclaration
    {
        public AzureLandscape AzureLandscape { get; set; }
        public List<ResourceCollection> ResourceCollections { get; set; }
    }
}