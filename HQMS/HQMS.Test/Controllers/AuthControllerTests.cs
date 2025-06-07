using HospitalQueueSystem.Web.Controllers;
using HospitalQueueSystem.Web.Interfaces;
using HospitalQueueSystem.Web.Models;
using HQMS.Test.Helpers; // for MockSession
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace HQMS.Test.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["SignalR:HubUrl"]).Returns("https://fakehuburl");

            _controller = new AuthController(_mockAuthService.Object, _mockConfiguration.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();

            // Setup TempData with a fake provider
            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);
            _controller.TempData = tempData;

            // Setup authentication service
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(a => a.SignInAsync(
                    It.IsAny<HttpContext>(),
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            authServiceMock
                .Setup(a => a.SignOutAsync(
                    It.IsAny<HttpContext>(),
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    null))
                .Returns(Task.CompletedTask);

            var services = new Mock<IServiceProvider>();
            services.Setup(x => x.GetService(typeof(IAuthenticationService))).Returns(authServiceMock.Object);

            httpContext.RequestServices = services.Object;

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("https://localhost/Dashboard");
            _controller.Url = urlHelperMock.Object;
        }

        [Fact]
        public void Login_Get_ReturnsView()
        {
            var result = _controller.Login();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Login_Post_InvalidModel_ReturnsViewWithModel()
        {
            _controller.ModelState.AddModelError("Email", "Required");
            var model = new LoginModel();

            var result = await _controller.Login(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Login_Post_SuccessfulLogin_RedirectsToDashboardAndSetsSession()
        {
            var loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "password"
            };

            var tokenData = new TokenDto
            {
                Token = "fake.jwt.token",
                UserId = "user123",
                Role = "Admin",
                Expiration = DateTime.UtcNow.AddHours(1)
            };

            _mockAuthService.Setup(s => s.LoginAsync(loginModel))
                .ReturnsAsync((true, tokenData, null));

            var result = await _controller.Login(loginModel);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Dashboard", redirectResult.ControllerName);

            var session = _controller.HttpContext.Session;
            Assert.Equal("fake.jwt.token", session.GetString("JwtToken")); // <-- Using built-in extension
            Assert.Equal("test@example.com", session.GetString("UserEmail"));
            Assert.Equal("user123", session.GetString("UserName"));
            Assert.Equal("Admin", session.GetString("UserRole"));
        }

        [Fact]
        public async Task Login_Post_FailedLogin_ReturnsViewWithError()
        {
            var loginModel = new LoginModel
            {
                Email = "fail@example.com",
                Password = "wrongpass"
            };

            _mockAuthService.Setup(s => s.LoginAsync(loginModel))
                .ReturnsAsync((false, null, "Invalid credentials"));

            var result = await _controller.Login(loginModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey(""));
        }

        [Fact]
        public async Task Logout_Post_ClearsSessionAndSignsOut()
        {
            var result = await _controller.Logout();

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);

            Assert.Null(_controller.HttpContext.Session.GetString("JwtToken")); // built-in extension
        }
    }
}
