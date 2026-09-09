using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System;

namespace ClinicFlow.IntegrationTests.Infrastructure
{
    public static class AuthenticationHelper
    {
        public static async Task<HttpClient> GetSuperAdminClientAsync(CustomWebApplicationFactory factory)
        {
            return await CreateAuthenticatedClientAsync(factory, TestCredentials.SuperAdminEmail, TestCredentials.SuperAdminPassword);
        }

        public static async Task<HttpClient> GetClinicAOwnerClientAsync(CustomWebApplicationFactory factory)
        {
            return await CreateAuthenticatedClientAsync(factory, TestCredentials.ClinicAOwnerEmail, TestCredentials.ClinicAOwnerPassword);
        }

        public static async Task<HttpClient> GetClinicADoctor1ClientAsync(CustomWebApplicationFactory factory)
        {
            return await CreateAuthenticatedClientAsync(factory, TestCredentials.ClinicADoctor1Email, TestCredentials.ClinicADoctor1Password);
        }

        public static async Task<HttpClient> GetClinicAReceptionistClientAsync(CustomWebApplicationFactory factory)
        {
            return await CreateAuthenticatedClientAsync(factory, TestCredentials.ClinicAReceptionistEmail, TestCredentials.ClinicAReceptionistPassword);
        }

        public static async Task<HttpClient> GetClinicBOwnerClientAsync(CustomWebApplicationFactory factory)
        {
            return await CreateAuthenticatedClientAsync(factory, TestCredentials.ClinicBOwnerEmail, TestCredentials.ClinicBOwnerPassword);
        }

        public static async Task<HttpClient> CreateAuthenticatedClientAsync(CustomWebApplicationFactory factory, string email, string password)
        {

            var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { HandleCookies = true, BaseAddress = new Uri("https://localhost") });
            var loginRequest = new { Email = email, Password = password };
            var response = await client.PostAsJsonAsync("/api/Authentication/login", loginRequest);
            
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"Login failed for {email}. Status: {response.StatusCode}. Details: {body}");
            }
            return client;
        }
    }
}


