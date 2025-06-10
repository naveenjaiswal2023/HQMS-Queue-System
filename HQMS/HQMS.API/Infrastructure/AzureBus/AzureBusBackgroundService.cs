using Azure.Messaging.ServiceBus;
using HospitalQueueSystem.Domain.Entities;
using HospitalQueueSystem.Domain.Events;
using HospitalQueueSystem.Infrastructure.SignalR;
using HQMS.API.Domain.Events;
using HQMS.API.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public class AzureBusBackgroundService : BackgroundService
{
    private readonly List<ServiceBusProcessor> _processors = new();
    private readonly ILogger<AzureBusBackgroundService> _logger;
    private readonly ServiceBusClient _client;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly List<TopicSubscriptionPair> _topicSubscriptionPairs;
    private readonly IDistributedCache _cache;
    private readonly INotificationService _notificationService;

    public AzureBusBackgroundService(
        INotificationService notificationService,
        ServiceBusClient client,
        IServiceScopeFactory serviceScopeFactory,
        IHubContext<NotificationHub> hubContext,
        List<TopicSubscriptionPair> topicSubscriptionPairs,
        ILogger<AzureBusBackgroundService> logger,
        IDistributedCache cache)
    {
        _notificationService = notificationService;
        _client = client;
        _serviceScopeFactory = serviceScopeFactory;
        _hubContext = hubContext;
        _topicSubscriptionPairs = topicSubscriptionPairs;
        _logger = logger;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AzureBusBackgroundService is starting...");

        foreach (var pair in _topicSubscriptionPairs)
        {
            var processor = _client.CreateProcessor(pair.TopicName, pair.SubscriptionName, new ServiceBusProcessorOptions());

            processor.ProcessMessageAsync += OnMessageReceived;
            processor.ProcessErrorAsync += ErrorHandler;

            _processors.Add(processor);

            _logger.LogInformation("Starting processor for topic: {Topic}, subscription: {Subscription}", pair.TopicName, pair.SubscriptionName);

            await processor.StartProcessingAsync(stoppingToken);
        }
    }

    private async Task OnMessageReceived(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        var subject = args.Message.Subject;
        _logger.LogInformation("OnMessageReceived triggered for subject: {Subject}", subject);
        _logger.LogDebug("Message body: {Body}", body);

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            switch (subject)
            {
                case nameof(PatientRegisteredEvent):
                    await HandleMessage<PatientRegisteredEvent>(body, args, "PatientRegisteredEvent", subject);
                    break;
                case nameof(PatientUpdatedEvent):
                    await HandleMessage<PatientUpdatedEvent>(body, args, "PatientUpdatedEvent", subject);
                    break;
                case nameof(PatientDeletedEvent):
                    await HandleMessage<PatientDeletedEvent>(body, args, "PatientDeletedEvent", subject);
                    break;
                case nameof(RoleCreatedEvent):
                    await HandleMessage<RoleCreatedEvent>(body, args, "PatientRegisteredEvent", subject);
                    break;
                case nameof(RoleUpdatedEvent):
                    await HandleMessage<RoleUpdatedEvent>(body, args, "PatientUpdatedEvent", subject);
                    break;
                case nameof(RoleDeletedEvent):
                    await HandleMessage<RoleDeletedEvent>(body, args, "PatientDeletedEvent", subject);
                    break;
                default:
                    _logger.LogWarning("Unknown message subject: {Subject}", subject);
                    await args.DeadLetterMessageAsync(args.Message, "Unknown subject", subject);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while processing message with subject: {Subject}", subject);
            await args.DeadLetterMessageAsync(args.Message, "Unhandled processing error", ex.Message);
        }
    }

    private async Task HandleMessage<T>(string body, ProcessMessageEventArgs args, string eventName, string subject)
    {
        try
        {
            var message = JsonSerializer.Deserialize<T>(body);
            if (message != null)
            {
                _logger.LogInformation("Sending notification for subject: {Subject} with message: {@Message}", subject, message);

                // FIX: Call the correct method that matches your JavaScript client
                await _notificationService.SendNotificationAsync("ReceiveNotification", eventName, message);

                _logger.LogInformation("SignalR SendAsync completed for event: {EventName}", eventName);
                await args.CompleteMessageAsync(args.Message);
            }
            else
            {
                _logger.LogWarning("Deserialization of {Type} resulted in null", typeof(T).Name);
                await args.DeadLetterMessageAsync(args.Message, "Deserialization failed", "Null result");
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Deserialization error for type {Type}", typeof(T).Name);
            await args.DeadLetterMessageAsync(args.Message, "JSON error", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message for subject: {Subject}", subject);
            await args.DeadLetterMessageAsync(args.Message, "Processing error", ex.Message);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception,
            "Service Bus Error. Entity: {EntityPath}, Operation: {ErrorSource}",
            args.EntityPath, args.ErrorSource);

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping AzureBusBackgroundService...");

        foreach (var processor in _processors)
        {
            if (processor == null)
                continue;

            try
            {
                _logger.LogInformation("Stopping processor for entity: {EntityPath}", processor.EntityPath);
                await processor.StopProcessingAsync(cancellationToken);
                await processor.DisposeAsync();
            }
            catch (ObjectDisposedException ex)
            {
                _logger.LogWarning(ex, "Processor for entity {EntityPath} was already disposed.", processor.EntityPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while stopping processor for entity {EntityPath}.", processor.EntityPath);
            }
        }

        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _logger.LogInformation("Disposing AzureBusBackgroundService...");

        foreach (var processor in _processors)
        {
            if (processor == null) continue;

            try
            {
                processor.DisposeAsync().AsTask().GetAwaiter().GetResult(); // Safe sync await
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while disposing processor.");
            }
        }

        base.Dispose();
    }
}
