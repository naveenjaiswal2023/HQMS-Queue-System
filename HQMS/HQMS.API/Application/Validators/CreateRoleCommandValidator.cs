using FluentValidation;
using HQMS.API.Application.CommandModel;

namespace HQMS.API.Application.Validators
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.")
                .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Role name can only contain letters, numbers, underscores, and dashes.");
        }
    }
}
