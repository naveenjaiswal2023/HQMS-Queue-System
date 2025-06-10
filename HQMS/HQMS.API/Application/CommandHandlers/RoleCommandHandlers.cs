using HQMS.API.Application.CommandModel;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HQMS.API.Application.CommandHandlers
{
    //public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, IdentityResult>
    //{
    //    private readonly RoleManager<IdentityRole> _roleManager;

    //    public CreateRoleCommandHandler(RoleManager<IdentityRole> roleManager)
    //    {
    //        _roleManager = roleManager;
    //    }

    //    public async Task<IdentityResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    //    {
    //        var role = new IdentityRole(request.RoleName);
    //        return await _roleManager.CreateAsync(role);
    //    }
    //}

    //public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IdentityResult>
    //{
    //    private readonly RoleManager<IdentityRole> _roleManager;

    //    public UpdateRoleCommandHandler(RoleManager<IdentityRole> roleManager)
    //    {
    //        _roleManager = roleManager;
    //    }

    //    public async Task<IdentityResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    //    {
    //        var role = await _roleManager.FindByIdAsync(request.RoleId);
    //        if (role == null) return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

    //        role.Name = request.NewRoleName;
    //        return await _roleManager.UpdateAsync(role);
    //    }
    //}

    //public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, IdentityResult>
    //{
    //    private readonly RoleManager<IdentityRole> _roleManager;

    //    public DeleteRoleCommandHandler(RoleManager<IdentityRole> roleManager)
    //    {
    //        _roleManager = roleManager;
    //    }

    //    public async Task<IdentityResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    //    {
    //        var role = await _roleManager.FindByIdAsync(request.RoleId);
    //        if (role == null) return IdentityResult.Failed(new IdentityError { Description = "Role not found." });

    //        return await _roleManager.DeleteAsync(role);
    //    }
    //}

}
