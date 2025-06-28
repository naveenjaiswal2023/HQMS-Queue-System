using MediatR;

namespace HQMS.Application.CommandModel
{
    public class CallPatientCommand : IRequest<bool>
    {
        public Guid QueueEntryId { get; set; }
    }
}
