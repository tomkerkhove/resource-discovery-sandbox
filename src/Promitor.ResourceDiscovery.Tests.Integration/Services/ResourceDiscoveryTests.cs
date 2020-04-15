using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Bogus;
using Newtonsoft.Json;
using Promitor.ResourceDiscovery.Agent.Model;
using Xunit;
using Xunit.Abstractions;

namespace Promitor.ResourceDiscovery.Tests.Integration.Services
{
    [Category("Integration")]
    public class ResourceDiscoveryTests : IntegrationTest
    {
        private readonly Faker _bogusGenerator = new Faker();

        public ResourceDiscoveryTests(ITestOutputHelper testOutput)
          : base(testOutput)
        {
        }

        [Fact]
        public async Task ResourceDiscovery_GetServiceBusResources_ReturnsValidList()
        {
            // Arrange
            const string resourceCollectionName = "service-bus";
            var resourceDiscoveryClient = new ResourceDiscoveryClient(Configuration, Logger);

            // Act
            var response = await resourceDiscoveryClient.GetDiscoveredResourcesAsync(resourceCollectionName);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var rawResponseBody = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(rawResponseBody);
            var resources = JsonConvert.DeserializeObject<List<Resource>>(rawResponseBody);
            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
        }

        [Fact]
        public async Task ResourceDiscovery_GetForUnexistingResourceCollection_ReturnsNotFound()
        {
            // Arrange
            string resourceCollectionName = _bogusGenerator.Lorem.Word();
            var resourceDiscoveryClient = new ResourceDiscoveryClient(Configuration, Logger);

            // Act
            var response = await resourceDiscoveryClient.GetDiscoveredResourcesAsync(resourceCollectionName);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
