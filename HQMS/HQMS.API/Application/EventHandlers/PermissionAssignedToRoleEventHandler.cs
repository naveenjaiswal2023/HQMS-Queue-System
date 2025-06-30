using Azure.Messaging.ServiceBus;
using HQMS.API.Domain.Events;
using MediatR;
using System.Text.Json;

namespace HQMS.API.Application.EventHandlers
{
    public class PermissionAssignedToRoleEventHandler : INotificationHandler<PermissionAssignedToRoleEvent>
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ILogger<PermissionAssignedToRoleEventHandler> _logger;
        private readonly string _topicName;

        public PermissionAssignedToRoleEventHandler(
            ServiceBusClient serviceBusClient,
            ILogger<PermissionAssignedToRoleEventHandler> logger,
            IConfiguration configuration)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _topicName = configuration["AzureServiceBus:QmsNotificationTopic"];
        }

        public async Task Handle(PermissionAssignedToRoleEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await using var sender = _serviceBusClient.CreateSender(_topicName);

                var messageBody = JsonSerializer.Serialize(notification);
                var message = new ServiceBusMessage(messageBody)
                {
                    Subject = nameof(PermissionAssignedToRoleEvent)
                };

                await sender.SendMessageAsync(message, cancellationToken);

                _logger.LogInformation("✅ Published PermissionAssignedToRoleEvent for RoleId: {RoleId}, PermissionId: {PermissionId}",
                    notification.RoleId, notification.PermissionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to publish PermissionAssignedToRoleEvent.");
            }
        }
    }
}
