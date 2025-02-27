﻿using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.AzureServiceBus
{
    public class AzureEventHubHealthCheck
        : IHealthCheck
    {
        const string EntityPathSegment = "EntityPath=";

        private static readonly ConcurrentDictionary<string, EventHubProducerClient> _eventHubConnections =
            new ConcurrentDictionary<string, EventHubProducerClient>();

        private readonly string _eventHubConnectionString;

        public AzureEventHubHealthCheck(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (!connectionString.Contains(EntityPathSegment))
            {
                throw new ArgumentException(
                    "Connection string should contain event hub name.",
                    nameof(connectionString)
                );
            }

            _eventHubConnectionString = connectionString;
        }

        public AzureEventHubHealthCheck(string connectionString, string eventHubName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (string.IsNullOrEmpty(eventHubName))
            {
                throw new ArgumentNullException(nameof(eventHubName));
            }

            _eventHubConnectionString = connectionString.Contains(EntityPathSegment)
                ? connectionString
                : $"{connectionString};{EntityPathSegment}{eventHubName}";
        }

        public AzureEventHubHealthCheck(EventHubConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _eventHubConnectionString =
                $"{connection.FullyQualifiedNamespace};{EntityPathSegment}{connection.EventHubName}";
            if (!_eventHubConnections.TryAdd(_eventHubConnectionString, new EventHubProducerClient(connection)))
            {
                throw new InvalidOperationException(
                    "EventHubProducerClient can't be created using the specified connection.");
            }
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (!_eventHubConnections.TryGetValue(_eventHubConnectionString, out var producer))
                {
                    producer = new EventHubProducerClient(_eventHubConnectionString);

                    if (!_eventHubConnections.TryAdd(_eventHubConnectionString, producer))
                    {
                        return new HealthCheckResult(context.Registration.FailureStatus,
                            description: "EventHubProducerClient can't be added into dictionary.");
                    }
                }

                _ = await producer.GetEventHubPropertiesAsync(cancellationToken);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
