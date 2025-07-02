using FluentValidation;
using HQMS.API.Domain.Entities;

namespace HQMS.API.Application.Validators
{
    public class DepartmentValidator : AbstractValidator<Department>
    {
        public DepartmentValidator()
        {
            RuleFor(d => d.DepartmentId)
                .NotEmpty().WithMessage("Department ID is required.");

            RuleFor(d => d.DepartmentName)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(100).WithMessage("Department name must be less than 100 characters.");

            RuleFor(d => d.HospitalId)
                .NotNull().WithMessage("Hospital ID is required.");
        }
    }
}
