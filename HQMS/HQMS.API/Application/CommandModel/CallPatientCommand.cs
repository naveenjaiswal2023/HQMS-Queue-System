using MediatR;

namespace HospitalQueueSystem.Application.CommandModel
{
    public class CallPatientCommand : IRequest<bool>
    {
        public Guid QueueEntryId { get; set; }
    }
}
