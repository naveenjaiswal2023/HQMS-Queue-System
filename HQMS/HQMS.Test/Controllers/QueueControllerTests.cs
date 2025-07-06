using HQMS.Tests.Common;
using HQMS.UI.Controllers;
using HQMS.UI.Interfaces;
using HQMS.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace HQMS.Tests.Controllers
{
    public class QueueControllerTests : ControllerTestBase
    {
        private readonly Mock<IQueueService> _mockQueueService;
        private readonly QueueController _controller;

        public QueueControllerTests()
        {
            _mockQueueService = new Mock<IQueueService>();
            _controller = new QueueController(_mockQueueService.Object);
            SetUserContext(_controller);
        }

        [Fact]
        public async Task Index_ReturnsViewWithDashboardData()
        {
            // Arrange
            var hospitalId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();

            var dashboardData = new List<QueueDashboardItemDto>
            {
                new QueueDashboardItemDto
                {
                    QueueNumber = "Q-101",
                    PatientName = "John Doe",
                    DoctorName = "Dr. A",
                    Department = "Cardiology",
                    HospitalName = "Fortis"
                }
            };

            _mockQueueService
                .Setup(s => s.GetDashboardAsync(hospitalId, departmentId, doctorId))
                .ReturnsAsync(dashboardData);

            // Act
            var result = await _controller.Index(hospitalId, departmentId, doctorId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<QueueDashboardItemDto>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal("Q-101", model[0].QueueNumber);
        }

        [Fact]
        public async Task CallQueue_Post_SetsTempDataAndRedirects()
        {
            // Arrange
            string queueNumber = "Q-123";

            // Need to mock TempData
            var tempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());

            _controller.TempData = tempData;

            // Act
            var result = await _controller.CallQueue(queueNumber);

            // Assert
            Assert.True(_controller.TempData.ContainsKey("Success"));
            Assert.Equal($"Queue {queueNumber} called successfully!", _controller.TempData["Success"]);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
