using HQMS.Tests.Common;
using HQMS.Web.Controllers;
using HQMS.Web.Interfaces;
using HQMS.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HQMS.Tests.Controllers
{
    public class PatientControllerTests : ControllerTestBase
    {
        private readonly Mock<IPatientService> _mockService;
        private readonly PatientController _controller;

        public PatientControllerTests()
        {
            _mockService = new Mock<IPatientService>();
            _controller = new PatientController(_mockService.Object);
            SetUserContext(_controller);
        }

        [Fact]
        public async Task Index_ReturnsViewWithPatients()
        {
            // Arrange
            var patients = new List<PatientModel> {
                new PatientModel { PatientId = Guid.NewGuid(), Name = "John Doe" }
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(patients);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(patients, viewResult.Model);
        }

        [Fact]
        public void Create_Get_ReturnsView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            var model = new PatientModel
            {
                Name = "Jane Doe",
                Age = 30,
                Gender = "Female",
                Department = "Cardiology",
                PhoneNumber = "9876543210",
                Email = "jane@domain.com",
                Address = "New Street",
                BloodGroup = "B+",
                HospitalId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid()
            };

            var result = await _controller.Create(model);

            _mockService.Verify(s => s.CreateAsync(It.IsAny<PatientModel>()), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_Get_PatientFound_ReturnsView()
        {
            var id = Guid.NewGuid();
            var patient = new PatientModel { PatientId = id, Name = "Test Patient" };

            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(patient);

            var result = await _controller.Edit(id);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(patient, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_PatientNotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((PatientModel)null);

            var result = await _controller.Edit(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_RedirectsToIndex()
        {
            var id = Guid.NewGuid();
            var model = new PatientModel
            {
                PatientId = id,
                Name = "Edit Patient",
                Age = 28,
                Gender = "Male",
                Department = "ENT",
                PhoneNumber = "1234567890",
                Email = "edit@domain.com",
                Address = "Somewhere",
                BloodGroup = "O+",
                HospitalId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid()
            };

            var result = await _controller.Edit(id, model);

            _mockService.Verify(s => s.UpdateAsync(id, model), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Delete_Get_PatientFound_ReturnsView()
        {
            var id = Guid.NewGuid();
            var patient = new PatientModel { PatientId = id, Name = "To Be Deleted" };

            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(patient);

            var result = await _controller.Delete(id);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(patient, viewResult.Model);
        }

        [Fact]
        public async Task Delete_Get_PatientNotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((PatientModel)null);

            var result = await _controller.Delete(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_DeletesPatient_AndRedirects()
        {
            var id = Guid.NewGuid();

            var result = await _controller.DeleteConfirmed(id);

            _mockService.Verify(s => s.DeleteAsync(id), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
    }
}
