using Azure.Messaging.ServiceBus;
using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HQMS.API.Application.EventHandlers
{
    public class AssignMenusToRoleEventHandler : INotificationHandler<MenusAssignedToRoleEvent>
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ILogger<AssignMenusToRoleEventHandler> _logger;
        private readonly string _topicName;

        public AssignMenusToRoleEventHandler(ServiceBusClient serviceBusClient, ILogger<AssignMenusToRoleEventHandler> logger, IConfiguration configuration)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _topicName = configuration["AzureServiceBus:QmsNotificationTopic"];
        }

        public async Task Handle(MenusAssignedToRoleEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var sender = _serviceBusClient.CreateSender(_topicName);

                var messageBody = JsonSerializer.Serialize(notification);
                var message = new ServiceBusMessage(messageBody)
                {
                    Subject = nameof(MenusAssignedToRoleEvent)
                };

                await sender.SendMessageAsync(message, cancellationToken);

                _logger.LogInformation("✅ Published MenusAssignedToRoleEvent for RoleId: {RoleId}, MenuCount: {MenuCount}",
                    notification.RoleId, notification.MenuIds?.Count() ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to publish MenusAssignedToRoleEvent.");
            }
        }
    }
}
