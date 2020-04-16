﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Promitor.ResourceDiscovery.Agent.Configuration;
using Promitor.ResourceDiscovery.Agent.Graph;
using Promitor.ResourceDiscovery.Agent.Graph.Exceptions;

namespace Promitor.ResourceDiscovery.Agent.Health
{
    public class AzureResourceGraphHealthCheck : IHealthCheck
    {
        private readonly IOptionsMonitor<ResourceDeclaration> _resourceDeclarationMonitor;
        private readonly ILogger<AzureResourceGraphHealthCheck> _logger;
        private readonly AzureResourceGraph _azureResourceGraph;

        public AzureResourceGraphHealthCheck(AzureResourceGraph azureResourceGraph, IOptionsMonitor<ResourceDeclaration> resourceDeclarationMonitor, ILogger<AzureResourceGraphHealthCheck> logger)
        {
            Guard.NotNull(resourceDeclarationMonitor, nameof(resourceDeclarationMonitor));
            Guard.NotNull(azureResourceGraph, nameof(azureResourceGraph));
            Guard.NotNull(logger, nameof(logger));

            _logger = logger;
            _azureResourceGraph = azureResourceGraph;
            _resourceDeclarationMonitor = resourceDeclarationMonitor;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var query = GraphQuery.ForResourceType("microsoft.logic/workflows")
                .Project("subscriptionId", "resourceGroup", "type", "name", "id")
                .LimitTo(1)
                .Build();

            if (_resourceDeclarationMonitor.CurrentValue.AzureLandscape?.Subscriptions == null
                || _resourceDeclarationMonitor.CurrentValue.AzureLandscape?.Subscriptions?.Any() == false)
            {
                return HealthCheckResult.Unhealthy("No Azure subscriptions are configured");
            }

            var healthProbeResults = await ProbeSubscriptionsAsync(query);
            var healtCheckMetadata = GenerateMetadata(healthProbeResults);

            if (healthProbeResults.Any(probeKeyValuePair => probeKeyValuePair.IsSuccessful == false))
            {
                return HealthCheckResult.Unhealthy("One or more subscriptions are unhealthy.", data: healtCheckMetadata);
            }

            return HealthCheckResult.Healthy("Successfully queried all subscriptions", data: healtCheckMetadata);
        }

        private static Dictionary<string, object> GenerateMetadata(List<HealthProbeResult> healthProbeResults)
        {
            var healthResultMetadata = new Dictionary<string, object>();
            healthProbeResults.ForEach(result => healthResultMetadata.Add($"Subscription '{result.Subscription}'", new { IsSuccessful = result.IsSuccessful, Message = result.Description }));

            return healthResultMetadata;
        }

        private async Task<List<HealthProbeResult>> ProbeSubscriptionsAsync(string query)
        {
            List<HealthProbeResult> healthProbeResults = new List<HealthProbeResult>();

            foreach (var subscription in _resourceDeclarationMonitor.CurrentValue.AzureLandscape.Subscriptions)
            {
                var probeResult = await ProbeSubscriptionAsync(query, subscription);
                healthProbeResults.Add(probeResult);
            }

            return healthProbeResults;
        }

        private async Task<HealthProbeResult> ProbeSubscriptionAsync(string query, string subscription)
        {
            try
            {
                await _azureResourceGraph.QueryAsync(query, new List<string> { subscription });
                return new HealthProbeResult(subscription, isSuccessful: true, "Successfully queried resources via Azure Resource Graph");
            }
            catch (UnauthorizedException unauthorizedException)
            {
                return HandleFailedHealthProbe(subscription, unauthorizedException, unauthorizedException.Message);
            }
            catch (QueryContainsInvalidSubscriptionException queryContainsInvalidSubscriptionException)
            {
                return HandleFailedHealthProbe(subscription, queryContainsInvalidSubscriptionException, queryContainsInvalidSubscriptionException.Message);
            }
            catch (Exception exception)
            {
                var message = $"Unable to discover resources due to an unexpected exception: {exception.Message}";

                return HandleFailedHealthProbe(subscription, exception, message);
            }
        }

        private HealthProbeResult HandleFailedHealthProbe(string subscription, Exception exception, string message)
        {
            _logger.LogCritical(exception, message);
            return new HealthProbeResult(subscription, false, message);
        }
    }
}
