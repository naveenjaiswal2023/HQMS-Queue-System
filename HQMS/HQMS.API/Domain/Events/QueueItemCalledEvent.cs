using HQMS.Domain.Common;
using MediatR;

namespace HQMS.API.Domain.Events
{
    public class QueueItemCalledEvent : IDomainEvent, INotification
    {
        public Guid QueueItemId { get; }

        public QueueItemCalledEvent(Guid queueItemId)
        {
            QueueItemId = queueItemId;
            //OccurredOn = DateTime.UtcNow;
        }
    }
}
