using HQMS.Application.CommandModel;
using HQMS.Application.Common;
using HQMS.Domain.Interfaces;
using HQMS.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HQMS.Application.CommandHandlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterCommand, ICommandResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ICommandResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return CommandResult.Failure(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "POD");

            return CommandResult.Success("User created successfully");
        }
    }
}
