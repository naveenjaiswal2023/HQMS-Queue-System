using HQMS.Domain.Events;
using MediatR;

namespace HQMS.API.Application.QueriesModel
{
    public class GetPatientByIdQuery : IRequest<List<PatientUpdatedEvent>>
    {
        public Guid PatientId { get; }

        public GetPatientByIdQuery(Guid patientId)
        {
            PatientId = patientId;
        }
    }
}
