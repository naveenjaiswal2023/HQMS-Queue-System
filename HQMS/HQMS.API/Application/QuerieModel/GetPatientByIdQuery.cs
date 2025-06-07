using HospitalQueueSystem.Domain.Events;
using MediatR;

namespace HQMS.API.Application.QuerieModel
{
    public class GetPatientByIdQuery : IRequest<List<PatientUpdatedEvent>>
    {
        public string PatientId { get; }

        public GetPatientByIdQuery(string patientId)
        {
            PatientId = patientId;
        }
    }
}
