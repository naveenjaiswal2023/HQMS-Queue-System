using HospitalQueueSystem.Domain.Events;
using MediatR;

namespace HQMS.API.Application.QuerieModel
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
