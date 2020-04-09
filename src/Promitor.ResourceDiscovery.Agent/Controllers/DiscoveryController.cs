using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GuardNet;

namespace Promitor.ResourceDiscovery.Agent.Controllers
{
    /// <summary>
    /// API endpoint to discover Azure resources
    /// </summary>
    [ApiController]
    [Route("api/v1/discovery")]
    public class DiscoveryController : ControllerBase
    {
        private readonly AzureResourceGraph _azureResourceGraph;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryController"/> class.
        /// </summary>
        public DiscoveryController(AzureResourceGraph azureResourceGraph)
        {
            Guard.NotNull(azureResourceGraph, nameof(azureResourceGraph));

            _azureResourceGraph = azureResourceGraph;
        }

        /// <summary>
        ///     Discover Resources
        /// </summary>
        /// <remarks>Discovers Azure resources matching the criteria.</remarks>
        [HttpGet(Name = "Discovery_Get")]
        public async Task<IActionResult> Get()
        {
            var foundResources = await _azureResourceGraph.QueryAsync();
            return Ok(foundResources);
        }
    }
}
