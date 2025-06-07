using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using HQMS.UI;
namespace HQMS.Test.Integration
{
    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_Post_WithValidCredentials_ShouldRedirectToDashboard()
        {
            // Arrange
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", "admin@qms.com"),
                new KeyValuePair<string, string>("Password", "Admin@123")
            });

            // Act
            var response = await _client.PostAsync("/Auth/Login", formData);

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("DEBUG STATUS: " + response.StatusCode);
            Console.WriteLine("DEBUG CONTENT: " + content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Expect 302
            Assert.Equal("/Dashboard/Index", response.Headers.Location?.ToString());
        }

        [Fact]
        public async Task Login_Post_WithInvalidCredentials_ShouldReturnLoginViewWithError()
        {
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", "invalid@qms.com"),
                new KeyValuePair<string, string>("Password", "wrongpass")
            });

            var response = await _client.PostAsync("/Auth/Login", formData);
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Still renders login view
            Assert.Contains("Invalid credentials", content); // From TempData or ViewModel
        }

    }
}