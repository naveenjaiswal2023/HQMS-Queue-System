using Azure.Messaging.ServiceBus;
using HQMS.Domain.Events;
using MediatR;
using System.Text.Json;

namespace HQMS.Application.EventHandlers
{
    public class PatientDeletedEventHandler : INotificationHandler<PatientDeletedEvent>
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ILogger<PatientDeletedEventHandler> _logger;
        private readonly string _topicName;

        public PatientDeletedEventHandler(ServiceBusClient serviceBusClient, ILogger<PatientDeletedEventHandler> logger,IConfiguration configuration)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _topicName = configuration["AzureServiceBus:QmsNotificationTopic"];
        }

        public async Task Handle(PatientDeletedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var sender = _serviceBusClient.CreateSender(_topicName);

                var messageBody = JsonSerializer.Serialize(notification);
                var message = new ServiceBusMessage(messageBody)
                {
                    Subject = "PatientDeletedEvent"
                };

                await sender.SendMessageAsync(message, cancellationToken);
                _logger.LogInformation("Published PatientDeletedEvent for PatientId: {PatientId}", notification.PatientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish PatientDeletedEvent.");
            }
        }
    }
}
