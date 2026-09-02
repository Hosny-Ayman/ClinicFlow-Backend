using ClinicFlow.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace ClinicFlow.IntegrationTests.Authentication
{
    public class AuthenticationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthenticationIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient( new WebApplicationFactoryClientOptions
            {
                HandleCookies = true,
                BaseAddress = new Uri("https://localhost")
            });
        }

        [Fact]
        public async Task Login_WithValidCredentials_Returns200()
        {
            // Arrange
            var request = new
            {
                email = "string2@g",
                password = "stringstring"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Authentication/login", request);

            // Assert
            Assert.Contains(response.Headers.GetValues("Set-Cookie"), cookie => cookie.StartsWith("AccessToken="));

         
        }
    }
}