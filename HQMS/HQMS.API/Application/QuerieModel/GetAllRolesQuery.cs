using HospitalQueueSystem.Domain.Events;
using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HQMS.API.Application.QuerieModel
{

    public class GetAllRolesQuery : IRequest<List<RoleCreatedEvent>>
    {

    }

}
