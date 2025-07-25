﻿using Azure.Messaging.ServiceBus;
using HQMS.API.Domain.Events;
using MediatR;
using System.Text.Json;

namespace HQMS.API.Application.EventHandlers
{
    public class RoleUpdatedEventHandler : INotificationHandler<RoleUpdatedEvent>
    {
        private readonly ILogger<RoleUpdatedEventHandler> _logger;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly string _topicName;
        public RoleUpdatedEventHandler(ServiceBusClient serviceBusClient, ILogger<RoleUpdatedEventHandler> logger, IConfiguration configuration)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _topicName = configuration["AzureServiceBus:QmsNotificationTopic"];
        }

        public async Task Handle(RoleUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var sender = _serviceBusClient.CreateSender(_topicName);

                var messageBody = JsonSerializer.Serialize(notification);
                var message = new ServiceBusMessage(messageBody)
                {
                    Subject = "RoleUpdatedEvent"
                };

                await sender.SendMessageAsync(message, cancellationToken);
                _logger.LogInformation($"Role updated: ID = {notification.RoleId}, New Name = {notification.NewRoleName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RoleUpdatedEvent.");
            }
            
        }
    }

}
