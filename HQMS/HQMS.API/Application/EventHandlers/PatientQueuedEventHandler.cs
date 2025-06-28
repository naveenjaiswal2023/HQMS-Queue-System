using Azure.Messaging.ServiceBus;
using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace HQMS.API.Application.EventHandlers
{
    public class PatientQueuedEventHandler : INotificationHandler<PatientQueuedEvent>
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly string _topicName;
        private readonly ILogger<PatientQueuedEventHandler> _logger;

        public PatientQueuedEventHandler(
            ServiceBusClient serviceBusClient,
            ILogger<PatientQueuedEventHandler> logger,
            IConfiguration configuration)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _topicName = configuration["AzureServiceBus:QmsNotificationTopic"];
        }

        public async Task Handle(PatientQueuedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var sender = _serviceBusClient.CreateSender(_topicName);

                var messageBody = JsonSerializer.Serialize(notification);
                var message = new ServiceBusMessage(messageBody)
                {
                    Subject = nameof(PatientQueuedEvent)
                };

                await sender.SendMessageAsync(message, cancellationToken);

                _logger.LogInformation("PatientQueuedEvent published successfully for PatientId: {PatientId}, QueueId: {QueueId}",
                    notification.PatientId, notification.QueueItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish PatientQueuedEvent to Azure Service Bus.");
            }
        }
    }
}