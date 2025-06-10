using Azure.Messaging.ServiceBus;
using HospitalQueueSystem.Application.EventHandlers;
using HQMS.API.Domain.Events;
using MediatR;
using System.Text.Json;

namespace HQMS.API.Application.EventHandlers
{
    public class RoleCreatedEventHandler : INotificationHandler<RoleCreatedEvent>
    {
        private readonly ServiceBusClient _serviceBusClient;
        private const string TopicName = "patient-topic";
        private readonly ILogger<RoleCreatedEventHandler> _logger;

        public RoleCreatedEventHandler(ServiceBusClient serviceBusClient, ILogger<RoleCreatedEventHandler> logger)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
        }

        public async Task Handle(RoleCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var sender = _serviceBusClient.CreateSender(TopicName);

                var messageBody = JsonSerializer.Serialize(notification);
                var message = new ServiceBusMessage(messageBody)
                {
                    Subject = "RoleCreatedEvent"
                };

                await sender.SendMessageAsync(message, cancellationToken);
                _logger.LogInformation($"Role created: ID = {notification.RoleId}, Name = {notification.RoleName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RoleCreatedEvent.");
            }
            
            
        }
    }

}
