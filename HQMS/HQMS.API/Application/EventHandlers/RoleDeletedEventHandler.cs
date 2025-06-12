using Azure.Messaging.ServiceBus;
using HospitalQueueSystem.Application.EventHandlers;
using HQMS.API.Domain.Events;
using MediatR;
using System.Text.Json;

namespace HQMS.API.Application.EventHandlers
{
    public class RoleDeletedEventHandler : INotificationHandler<RoleDeletedEvent>
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ILogger<RoleDeletedEventHandler> _logger;
        private readonly string _topicName;


        public RoleDeletedEventHandler(ServiceBusClient serviceBusClient, ILogger<RoleDeletedEventHandler> logger,IConfiguration configuration)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _topicName = configuration["AzureServiceBus:QmsNotificationTopic"];
        }

        public async Task Handle(RoleDeletedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var sender = _serviceBusClient.CreateSender(_topicName);

                var messageBody = JsonSerializer.Serialize(notification);
                var message = new ServiceBusMessage(messageBody)
                {
                    Subject = "RoleDeletedEvent"
                };

                await sender.SendMessageAsync(message, cancellationToken);
                _logger.LogInformation($"Role deleted: ID = {notification.RoleId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RoleDeletedEvent.");
            }
            
            
        }
    }

}
