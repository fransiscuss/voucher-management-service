using System.Text.Json;
using Acme.Services.VoucherManagementService.Application.Interfaces.Infrastructure;
using Acme.Services.VoucherManagementService.Application.Models.Configuration;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Acme.Services.VoucherManagementService.Infrastructure.Messaging;

public class MessagePublishService : IMessagePublishService
{
    private readonly IAzureClientFactory<ServiceBusClient> _serviceBusClientFactory;
    private readonly ILogger<MessagePublishService> _logger;
    private readonly ServiceBusSettings _settings;
    private readonly AsyncRetryPolicy _retryPolicy;

    public MessagePublishService(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IOptions<ServiceBusSettings> settings,
        ILogger<MessagePublishService> logger)
    {
        _serviceBusClientFactory = serviceBusClientFactory;
        _logger = logger;
        _settings = settings.Value;

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, 
                        "Error publishing message to Service Bus. Retry attempt {RetryCount} after {TimeSpan}...",
                        retryCount, timeSpan);
                }
            );
    }

    public async Task SendMessage<T>(string topicName, T message, IDictionary<string, object>? applicationProperties = null)
    {
        try
        {
            // Get the primary service bus client
            var client = _serviceBusClientFactory.CreateClient("TokenRedeemServiceBusClient");
            
            await SendMessageWithClient(client, topicName, message, applicationProperties);

            // If alternate region is enabled, also publish to it
            if (_settings.IsAlternateRegionEnabled)
            {
                try
                {
                    var alternateClient = _serviceBusClientFactory.CreateClient("TokenRedeemAlternateServiceBusClient");
                    await SendMessageWithClient(alternateClient, topicName, message, applicationProperties);
                }
                catch (Exception ex)
                {
                    // Log but don't fail if alternate region publishing fails
                    _logger.LogError(ex, "Failed to publish message to alternate region");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to Service Bus");
            throw;
        }
    }

    private async Task SendMessageWithClient<T>(ServiceBusClient client, string topicName, T message, IDictionary<string, object>? applicationProperties)
    {
        // Apply retry policy
        await _retryPolicy.ExecuteAsync(async () =>
        {
            // Serialize the message to JSON
            var messageJson = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(messageJson)
            {
                ContentType = "application/json"
            };

            // Add application properties if specified
            if (applicationProperties != null)
            {
                foreach (var property in applicationProperties)
                {
                    serviceBusMessage.ApplicationProperties.Add(property.Key, property.Value);
                }
            }

            // Create sender and send message
            await using var sender = client.CreateSender(topicName);
            _logger.LogInformation("Sending message to topic: {TopicName}", topicName);
            
            await sender.SendMessageAsync(serviceBusMessage);
            _logger.LogInformation("Message successfully sent to topic: {TopicName}", topicName);
        });
    }
}
