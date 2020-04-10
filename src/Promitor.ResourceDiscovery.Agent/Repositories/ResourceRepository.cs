﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.Extensions.Options;
using Promitor.ResourceDiscovery.Agent.Configuration;
using Promitor.ResourceDiscovery.Agent.Controllers;
using Promitor.ResourceDiscovery.Agent.Graph;
using Promitor.ResourceDiscovery.Agent.Model;

namespace Promitor.ResourceDiscovery.Agent.Repositories
{
    public class ResourceRepository
    {
        private readonly AzureResourceGraph _azureResourceGraph;
        private readonly IOptionsMonitor<ResourceDeclaration> _resourceDeclarationMonitor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscoveryController" /> class.
        /// </summary>
        public ResourceRepository(AzureResourceGraph azureResourceGraph, IOptionsMonitor<ResourceDeclaration> resourceDeclarationMonitor)
        {
            Guard.NotNull(resourceDeclarationMonitor, nameof(resourceDeclarationMonitor));
            Guard.NotNull(azureResourceGraph, nameof(azureResourceGraph));

            _azureResourceGraph = azureResourceGraph;
            _resourceDeclarationMonitor = resourceDeclarationMonitor;
        }

        /// <summary>
        ///     Get resources that are part of a given resource collection
        /// </summary>
        /// <param name="resourceCollectionName">Name of the resource collection</param>
        public async Task<List<Resource>> GetResourcesAsync(string resourceCollectionName)
        {
            var resourceDeclaration = _resourceDeclarationMonitor.CurrentValue;
            var resourceCollectionRequirement = resourceDeclaration.ResourceCollections.SingleOrDefault(coll =>
                coll.Name.Equals(resourceCollectionName, StringComparison.InvariantCultureIgnoreCase));
            var foundResources = await _azureResourceGraph.QueryAsync(resourceCollectionRequirement.Type);
            return foundResources;
        }
    }
}