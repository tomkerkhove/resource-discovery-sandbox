using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GuardNet;
using Microsoft.Extensions.Options;
using Promitor.ResourceDiscovery.Agent.Configuration;

namespace Promitor.ResourceDiscovery.Agent.Controllers
{
    /// <summary>
    /// API endpoint to interact with resource collections
    /// </summary>
    [ApiController]
    [Route("api/v1/resources/collections")]
    public class ResourceCollectionsController : ControllerBase
    {
        private readonly IOptionsMonitor<ResourceDeclaration> _resourceDeclarationMonitor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscoveryController" /> class.
        /// </summary>
        public ResourceCollectionsController(IOptionsMonitor<ResourceDeclaration> resourceDeclarationMonitor)
        {
            Guard.NotNull(resourceDeclarationMonitor, nameof(resourceDeclarationMonitor));

            _resourceDeclarationMonitor = resourceDeclarationMonitor;
        }

        /// <summary>
        ///     Get Resource Collections
        /// </summary>
        /// <remarks>Lists all available resource collections.</remarks>
        [HttpGet(Name = "ResourceCollections_Get")]
        public async Task<IActionResult> Get()
        {
            var resourceDeclaration = _resourceDeclarationMonitor.CurrentValue;
            return Ok(resourceDeclaration.ResourceCollections);
        }
    }
}
