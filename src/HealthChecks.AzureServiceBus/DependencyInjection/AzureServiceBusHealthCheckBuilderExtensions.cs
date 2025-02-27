﻿using Azure.Messaging.EventHubs;
using HealthChecks.AzureServiceBus;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureServiceBusHealthCheckBuilderExtensions
    {
        const string AZUREEVENTHUB_NAME = "azureeventhub";
        const string AZUREQUEUE_NAME = "azurequeue";
        const string AZURETOPIC_NAME = "azuretopic";
        const string AZURESUBSCRIPTION_NAME = "azuresubscription";

        /// <summary>
        /// Add a health check for specified Azure Event Hub.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="connectionString">The azure event hub connection string.</param>
        /// <param name="eventHubName">The azure event hub name. Can alternatively be provided as the EntityPath in the <paramref name="connectionString"/>.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'azureeventhub' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
        /// <param name="requiresSession">An optional boolean flag that indicates whether session is enabled on the queue or not. Defaults to false.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddAzureEventHub(
            this IHealthChecksBuilder builder,
            string connectionString,
            string eventHubName = default,
            string name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string> tags = default,
            TimeSpan? timeout = default
        )
        {
            var healthCheckRegistration = new HealthCheckRegistration(
                name ?? AZUREEVENTHUB_NAME,
                sp =>
                    eventHubName == default
                        ? new AzureEventHubHealthCheck(connectionString)
                        : new AzureEventHubHealthCheck(connectionString, eventHubName),
                failureStatus,
                tags,
                timeout);

            return builder.Add(healthCheckRegistration);
        }

        /// <summary>
        /// Add a health check for specified Azure Event Hub.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="eventHubConnectionFactory">The event hub connection factory used to create a event hub connection for this health check.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'azureeventhub' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
        /// <param name="requiresSession">An optional boolean flag that indicates whether session is enabled on the queue or not. Defaults to false.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddAzureEventHub(
            this IHealthChecksBuilder builder,
            Func<IServiceProvider, EventHubConnection> eventHubConnectionFactory,
            string name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string> tags = default,
            TimeSpan? timeout = default
        )
        {
            return builder.Add(new HealthCheckRegistration(
                name ?? AZUREEVENTHUB_NAME,
                sp => new AzureEventHubHealthCheck(eventHubConnectionFactory(sp)),
                failureStatus,
                tags,
                timeout));
        }

        /// <summary>
        /// Add a health check for specified Azure Service Bus Queue.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="connectionString">The azure service bus connection string to be used.</param>
        /// <param name="queueName">The name of the queue to check.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'azurequeue' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="configuringMessage">Message configuration Action, usually used when queue is partitioned or with duplication detection feature enabled Optional.</param>
        /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
        /// <param name="requiresSession">An optional boolean flag that indicates whether session is enabled on the queue or not. Defaults to false.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddAzureServiceBusQueue(
            this IHealthChecksBuilder builder,
            string connectionString,
            string queueName,
            string name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string> tags = default,
            TimeSpan? timeout = default
        )
        {
            return builder.Add(new HealthCheckRegistration(
                name ?? AZUREQUEUE_NAME,
                sp => new AzureServiceBusQueueHealthCheck(connectionString, queueName),
                failureStatus,
                tags,
                timeout));
        }

        /// <summary>
        /// Add a health check for Azure Service Bus Topic.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="connectionString">The Azure ServiceBus connection string to be used.</param>
        /// <param name="topicName">The topic name of the topic to check.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'azuretopic' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="configuringMessage">Message configuration Action, usually used when topic is partitioned or with duplication detection feature enabled. Optional.</param>
        /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddAzureServiceBusTopic(
            this IHealthChecksBuilder builder,
            string connectionString,
            string topicName,
            string name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string> tags = default,
            TimeSpan? timeout = default
        )
        {
            return builder.Add(new HealthCheckRegistration(
                name ?? AZURETOPIC_NAME,
                sp => new AzureServiceBusTopicHealthCheck(connectionString, topicName),
                failureStatus,
                tags,
                timeout));
        }

        /// <summary>
        /// Add a health check for Azure Service Bus Subscription.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="connectionString">The Azure ServiceBus connection string to be used.</param>
        /// <param name="topicName">The topic name of the topic to check.</param>
        /// <param name="subscriptionName">The subscription name of the topic to check.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'azuretopic' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="configuringMessage">Message configuration Action, usually used when topic is partitioned or with duplication detection feature enabled. Optional.</param>
        /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddAzureServiceBusSubscription(
            this IHealthChecksBuilder builder,
            string connectionString,
            string topicName,
            string subscriptionName,
            string name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string> tags = default,
            TimeSpan? timeout = default
        )
        {
            return builder.Add(new HealthCheckRegistration(
                name ?? AZURESUBSCRIPTION_NAME,
                sp => new AzureServiceBusSubscriptionHealthCheck(connectionString, topicName, subscriptionName),
                failureStatus,
                tags,
                timeout));
        }
    }
}