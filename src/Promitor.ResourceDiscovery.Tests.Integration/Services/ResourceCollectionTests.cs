using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Promitor.ResourceDiscovery.Agent.Configuration;
using Xunit;

namespace Promitor.ResourceDiscovery.Tests.Integration.Services
{
    [Category("Integration")]
    public class ResourceCollectionTests
    {
        [Fact]
        public async Task ResourceCollection_GetAll_ReturnsValidList()
        {
            // Arrange
            var resourceDiscoveryClient = new ResourceDiscoveryClient();
            
            // Act
            var response = await resourceDiscoveryClient.GetResourceCollectionsAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var rawResponseBody = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(rawResponseBody);
            var resourceCollections = JsonConvert.DeserializeObject<List<ResourceCollection>>(rawResponseBody);
            Assert.NotNull(resourceCollections);
            Assert.NotEmpty(resourceCollections);
        }
    }
}
