using FluentValidation;
using HQMS.Application.CommandModel;

namespace HQMS.API.Application.Validators
{
    public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientCommandValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient ID is required.")
                .Must(pid => Guid.TryParse(pid, out _)).WithMessage("Patient ID must be a valid GUID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Patient name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Age)
                .GreaterThan(0).WithMessage("Age must be greater than 0.")
                .LessThanOrEqualTo(120).WithMessage("Age must be realistic (<= 120).");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(g => new[] { "Male", "Female", "Other" }.Contains(g))
                .WithMessage("Gender must be Male, Female, or Other.");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department is required.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^[6-9]\d{9}$").WithMessage("Phone number must be a valid 10-digit Indian number.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200);

            RuleFor(x => x.BloodGroup)
                .NotEmpty().WithMessage("Blood group is required.")
                .Must(bg => new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" }.Contains(bg))
                .WithMessage("Invalid blood group.");

            RuleFor(x => x.HospitalId)
                .NotEqual(Guid.Empty).WithMessage("Hospital ID is required.");

            RuleFor(x => x.DoctorId)
                .NotEqual(Guid.Empty).WithMessage("Doctor ID is required.");

            RuleFor(x => x.UpdatedAt)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("UpdatedAt cannot be in the future.");
        }
    }
}
