using HQMS.Domain.Events;
using MediatR;

namespace HQMS.Application.Queries
{
    public class GetAllPatientsQuery : IRequest<List<PatientRegisteredEvent>>
    {

    }
}
