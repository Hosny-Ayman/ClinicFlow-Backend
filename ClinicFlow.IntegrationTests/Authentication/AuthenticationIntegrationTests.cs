using ClinicFlow.Application.Features.Authentication.DTOs.Requests;
using ClinicFlow.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ClinicFlow.IntegrationTests.Authentication
{
    public class AuthenticationIntegrationTests : IntegrationTestBase
    {
        public AuthenticationIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Login_WithValidCredentials_Returns200_AndSetsCookies()
        {
            var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                HandleCookies = true,
                BaseAddress = new Uri("https://localhost")
            });

            var loginRequest = new LoginDtoRequest
            {
                Email = TestCredentials.SuperAdminEmail,
                Password = TestCredentials.SuperAdminPassword
            };

            var response = await client.PostAsJsonAsync("/api/Authentication/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.True(root.GetProperty("isSuccess").GetBoolean());
            Assert.Equal("Login Successfully", root.GetProperty("data").GetString());

            var setCookieHeaders = response.Headers.GetValues("Set-Cookie").ToList();
            Assert.Contains(setCookieHeaders, cookie => cookie.StartsWith("AccessToken="));
            Assert.Contains(setCookieHeaders, cookie => cookie.StartsWith("RefreshToken="));
        }

        [Fact]
        public async Task Login_WithIncorrectPassword_ReturnsUnauthorized()
        {
            var client = Factory.CreateClient();
            var loginRequest = new LoginDtoRequest
            {
                Email = TestCredentials.SuperAdminEmail,
                Password = "WrongPassword123!"
            };

            var response = await client.PostAsJsonAsync("/api/Authentication/login", loginRequest);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.False(root.GetProperty("isSuccess").GetBoolean());
            
            var errors = root.GetProperty("errors").EnumerateArray().ToList();
            Assert.NotEmpty(errors);
            Assert.Contains("Email or Password is incorrect", errors[0].GetProperty("message").GetString());
        }

        [Fact]
        public async Task Login_WithNonExistingEmail_ReturnsUnauthorized()
        {
            var client = Factory.CreateClient();
            var loginRequest = new LoginDtoRequest
            {
                Email = "doesnotexist@g",
                Password = TestCredentials.SuperAdminPassword
            };

            var response = await client.PostAsJsonAsync("/api/Authentication/login", loginRequest);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.False(root.GetProperty("isSuccess").GetBoolean());
            
            var errors = root.GetProperty("errors").EnumerateArray().ToList();
            Assert.NotEmpty(errors);
            Assert.Contains("Email or Password is incorrect", errors[0].GetProperty("message").GetString());
        }

        [Fact]
        public async Task Login_WithInvalidRequestData_ReturnsBadRequest()
        {
            var client = Factory.CreateClient();
            var loginRequest = new LoginDtoRequest
            {
                Email = "invalidemailformat",
                Password = ""
            };

            var response = await client.PostAsJsonAsync("/api/Authentication/login", loginRequest);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AuthenticatedSession_CanAccessProtectedEndpoint_Returns200()
        {
            var client = await AuthenticationHelper.GetSuperAdminClientAsync(Factory);

            var response = await client.PostAsync("/api/Authentication/logout", null);

           
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
