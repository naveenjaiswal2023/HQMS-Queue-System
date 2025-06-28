using Azure.Messaging.ServiceBus;
using HQMS.Application.CommandModel;
using HQMS.Application.Common;
using HQMS.Application.Handlers;
using HQMS.Domain.Events;

using MediatR;
using System.Text.Json;

namespace HQMS.Application.EventHandlers
{
    public class PatientUpdateEventHandler : INotificationHandler<PatientUpdatedEvent>
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ILogger<PatientUpdateEventHandler> _logger;
        private readonly string _topicName;

        public PatientUpdateEventHandler(ServiceBusClient serviceBusClient, ILogger<PatientUpdateEventHandler> logger,IConfiguration configuration)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _topicName = configuration["AzureServiceBus:QmsNotificationTopic"];
        }

        public async Task Handle(PatientUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var sender = _serviceBusClient.CreateSender(_topicName);

                var messageBody = JsonSerializer.Serialize(notification);
                var message = new ServiceBusMessage(messageBody)
                {
                    Subject = "PatientUpdatedEvent"
                };

                await sender.SendMessageAsync(message, cancellationToken);
                _logger.LogInformation("Published PatientUpdatedEvent for PatientId: {PatientId}", notification.PatientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish PatientUpdatedEvent.");
            }
        }
    }
}
