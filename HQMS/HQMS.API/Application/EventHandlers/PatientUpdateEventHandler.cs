﻿using Azure.Messaging.ServiceBus;
using HospitalQueueSystem.Application.CommandModel;
using HospitalQueueSystem.Application.Common;
using HospitalQueueSystem.Application.Handlers;
using HospitalQueueSystem.Domain.Entities;
using HospitalQueueSystem.Domain.Events;
using HospitalQueueSystem.Domain.Interfaces;
using MediatR;
using System.Text.Json;

namespace HospitalQueueSystem.Application.EventHandlers
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
