using FluentValidation;
using HQMS.API.Domain.Entities;

namespace HQMS.API.Application.Validators
{
    public class HospitalValidator : AbstractValidator<Hospital>
    {
        public HospitalValidator()
        {
            RuleFor(h => h.HospitalId)
                .NotEmpty().WithMessage("Hospital ID is required.");

            RuleFor(h => h.Name)
                .NotEmpty().WithMessage("Hospital name is required.")
                .MaximumLength(100).WithMessage("Hospital name must be less than 100 characters.");

            RuleFor(h => h.Code)
                .NotEmpty().WithMessage("Hospital code is required.")
                .MaximumLength(10).WithMessage("Hospital code must be less than 10 characters.");

            RuleFor(h => h.Type)
                .NotEmpty().WithMessage("Hospital type is required.");

            // Address validation
            RuleFor(h => h.AddressLine1)
                .NotEmpty().WithMessage("Address Line 1 is required.");

            RuleFor(h => h.City)
                .NotEmpty().WithMessage("City is required.");

            RuleFor(h => h.State)
                .NotEmpty().WithMessage("State is required.");

            RuleFor(h => h.Country)
                .NotEmpty().WithMessage("Country is required.");

            RuleFor(h => h.PostalCode)
                .NotEmpty().WithMessage("Postal code is required.")
                .Matches(@"^\d{6}$").WithMessage("Postal code must be a 6-digit number.");

            // Contact Info
            RuleFor(h => h.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(h => h.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^[0-9]{10}$").WithMessage("Phone number must be 10 digits.");

            RuleFor(h => h.HelplineNumber)
                .NotEmpty().WithMessage("Helpline number is required.")
                .Matches(@"^[0-9]{10}$").WithMessage("Helpline number must be 10 digits.");

            // Capacity
            RuleFor(h => h.TotalBeds)
                .GreaterThanOrEqualTo(0).WithMessage("Total beds must be a non-negative number.");

            RuleFor(h => h.ICUUnits)
                .GreaterThanOrEqualTo(0).WithMessage("ICU units must be a non-negative number.");

            RuleFor(h => h.EmergencyUnits)
                .GreaterThanOrEqualTo(0).WithMessage("Emergency units must be a non-negative number.");
        }
    }
}
