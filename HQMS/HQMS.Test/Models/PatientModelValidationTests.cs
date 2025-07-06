using HQMS.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace HQMS.Tests.Models
{
    public class PatientModelValidationTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, context, results, validateAllProperties: true);
            return results;
        }

        [Fact]
        public void PatientModel_Invalid_WhenRequiredFieldsMissing()
        {
            // Arrange
            var model = new PatientModel(); // All fields missing

            // Act
            var results = ValidateModel(model);

            // Debug print for visual check
            foreach (var result in results)
                Console.WriteLine(result.ErrorMessage);

            // Assert
            var errorMessages = results.Select(r => r.ErrorMessage).ToList();

            Assert.Contains("Name is required", errorMessages);
            Assert.Contains("Gender is required", errorMessages);
            Assert.Contains("Department is required", errorMessages);
            Assert.Contains("Phone number is required", errorMessages);
            Assert.Contains("Email is required", errorMessages);
            Assert.Contains("Address is required", errorMessages);
            Assert.Contains("Blood group is required", errorMessages);
            Assert.Contains("Hospital is required", errorMessages);
            Assert.Contains("Doctor is required", errorMessages);
        }

        [Fact]
        public void PatientModel_Invalid_WhenAgeOutOfRange()
        {
            var model = new PatientModel
            {
                Name = "Test Patient",
                Age = 130, // Invalid
                Gender = "Male",
                Department = "Cardiology",
                PhoneNumber = "1234567890",
                Email = "test@domain.com",
                Address = "Test Address",
                BloodGroup = "O+",
                HospitalId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid()
            };

            var results = ValidateModel(model);

            Assert.Contains(results, r => r.ErrorMessage.Contains("Age must be between 0 and 120"));
        }

        [Fact]
        public void PatientModel_Invalid_WhenEmailFormatWrong()
        {
            var model = new PatientModel
            {
                Name = "Test",
                Age = 30,
                Gender = "Male",
                Department = "Neuro",
                PhoneNumber = "1234567890",
                Email = "invalid-email",
                Address = "Test Addr",
                BloodGroup = "A+",
                HospitalId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid()
            };

            var results = ValidateModel(model);

            Assert.Contains(results, r => r.ErrorMessage.Contains("Invalid email address"));
        }

        [Fact]
        public void PatientModel_Valid_WhenAllFieldsCorrect()
        {
            var model = new PatientModel
            {
                Name = "Valid Name",
                Age = 25,
                Gender = "Female",
                Department = "Ortho",
                PhoneNumber = "9876543210",
                Email = "valid@email.com",
                Address = "Valid Address",
                BloodGroup = "B+",
                HospitalId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid()
            };

            var results = ValidateModel(model);

            Assert.Empty(results); // No validation errors
        }
    }
}
