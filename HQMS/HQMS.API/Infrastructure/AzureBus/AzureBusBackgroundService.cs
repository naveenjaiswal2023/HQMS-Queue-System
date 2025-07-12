using Azure.Messaging.ServiceBus;
using HQMS.API.Application.Services;
using HQMS.API.Domain.Events;
using HQMS.API.Domain.Interfaces;
using HQMS.Domain.Entities;
using HQMS.Domain.Events;
using HQMS.Infrastructure.SignalR;
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
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly List<TopicSubscriptionPair> _topicSubscriptionPairs;
    private readonly ICacheService _cache;
    private readonly INotificationService _notificationService;

    private readonly Dictionary<string, Func<string, ProcessMessageEventArgs, Task>> _handlers;

    public AzureBusBackgroundService(
        INotificationService notificationService,
        ServiceBusClient client,
        IServiceScopeFactory serviceScopeFactory,
        IHubContext<NotificationHub> hubContext,
        List<TopicSubscriptionPair> topicSubscriptionPairs,
        ILogger<AzureBusBackgroundService> logger,
        ICacheService cache)
    {
        _notificationService = notificationService;
        _client = client;
        _scopeFactory = serviceScopeFactory;
        _hubContext = hubContext;
        _topicSubscriptionPairs = topicSubscriptionPairs;
        _logger = logger;
        _cache = cache;

        // Register event handlers
        _handlers = new()
        {
            [nameof(PatientRegisteredEvent)] = (body, args) => HandleMessage<PatientRegisteredEvent>(body, args),
            [nameof(PatientUpdatedEvent)] = (body, args) => HandleMessage<PatientUpdatedEvent>(body, args),
            [nameof(PatientDeletedEvent)] = (body, args) => HandleMessage<PatientDeletedEvent>(body, args),
            [nameof(PatientQueuedEvent)] = (body, args) => HandleMessage<PatientQueuedEvent>(body, args),
            [nameof(RoleCreatedEvent)] = (body, args) => HandleMessage<RoleCreatedEvent>(body, args),
            [nameof(RoleUpdatedEvent)] = (body, args) => HandleMessage<RoleUpdatedEvent>(body, args),
            [nameof(RoleDeletedEvent)] = (body, args) => HandleMessage<RoleDeletedEvent>(body, args),

        };
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
        var subject = args.Message.Subject;
        var body = args.Message.Body.ToString();

        _logger.LogInformation("Received message for subject: {Subject}", subject);
        _logger.LogDebug("Message body: {Body}", body);

        try
        {
            if (_handlers.TryGetValue(subject, out var handler))
            {
                await handler(body, args);
            }
            else
            {
                _logger.LogWarning("Unknown message subject: {Subject}", subject);
                await args.DeadLetterMessageAsync(args.Message, "Unknown subject", subject);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while processing message: {Subject}", subject);
            await args.DeadLetterMessageAsync(args.Message, "Unhandled processing error", ex.Message);
        }
    }

    private async Task HandleMessage<T>(string body, ProcessMessageEventArgs args)
    {
        var subject = args.Message.Subject;
        var eventName = typeof(T).Name;

        try
        {
            var message = JsonSerializer.Deserialize<T>(body);
            if (message is null)
            {
                _logger.LogWarning("Deserialization of {EventType} resulted in null.", eventName);
                await args.DeadLetterMessageAsync(args.Message, "Deserialization failed", "Null object");
                return;
            }

            // Cache invalidation logic
            if (subject == nameof(PatientRegisteredEvent))
            {
                await _cache.RemoveAsync("Patient_CacheKey");
            }
            else if (subject == nameof(PatientQueuedEvent))
            {
                await _cache.RemoveAsync("QueueDashboard_CacheKey");
            }

            // SignalR notification
            await _notificationService.SendNotificationAsync("ReceiveNotification", eventName, message);
            _logger.LogInformation("Notification sent for {EventType}.", eventName);

            await args.CompleteMessageAsync(args.Message);
        }
        catch (JsonException jex)
        {
            _logger.LogError(jex, "Deserialization error for type {Type}", eventName);
            await args.DeadLetterMessageAsync(args.Message, "JSON error", jex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message for subject: {Subject}", subject);
            await args.DeadLetterMessageAsync(args.Message, "Processing error", ex.Message);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Service Bus Error. Entity: {EntityPath}, Operation: {ErrorSource}", args.EntityPath, args.ErrorSource);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping AzureBusBackgroundService...");
        foreach (var processor in _processors)
        {
            if (processor == null) continue;

            try
            {
                _logger.LogInformation("Stopping processor for: {EntityPath}", processor.EntityPath);
                await processor.StopProcessingAsync(cancellationToken);
                await processor.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while stopping processor: {EntityPath}", processor.EntityPath);
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
                processor.DisposeAsync().AsTask().GetAwaiter().GetResult(); // safe sync wait
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while disposing processor.");
            }
        }

        base.Dispose();
    }
}
