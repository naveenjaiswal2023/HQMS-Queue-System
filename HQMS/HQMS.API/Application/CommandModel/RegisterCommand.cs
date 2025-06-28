using HQMS.Domain.Interfaces;
using MediatR;

namespace HQMS.Application.CommandModel
{
    public class RegisterCommand : IRequest<ICommandResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
