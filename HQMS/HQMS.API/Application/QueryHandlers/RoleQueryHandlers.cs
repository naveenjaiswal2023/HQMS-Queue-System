using HQMS.API.Application.QuerieModel;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HQMS.API.Application.QueryHandlers
{
    //public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<IdentityRole>>
    //{
    //    private readonly RoleManager<IdentityRole> _roleManager;

    //    public GetAllRolesQueryHandler(RoleManager<IdentityRole> roleManager)
    //    {
    //        _roleManager = roleManager;
    //    }

    //    public Task<List<IdentityRole>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    //    {
    //        return Task.FromResult(_roleManager.Roles.ToList());
    //    }
    //}

    //public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, IdentityRole>
    //{
    //    private readonly RoleManager<IdentityRole> _roleManager;

    //    public GetRoleByIdQueryHandler(RoleManager<IdentityRole> roleManager)
    //    {
    //        _roleManager = roleManager;
    //    }

    //    public async Task<IdentityRole> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    //    {
    //        return await _roleManager.FindByIdAsync(request.RoleId);
    //    }
    //}

}
