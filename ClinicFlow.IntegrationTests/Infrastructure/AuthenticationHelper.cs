using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace ClinicFlow.IntegrationTests.Infrastructure
{
    public class AuthenticationHelper
    {

        public static async Task<HttpClient> CreateAuthenticatedClientAsync(CustomWebApplicationFactory factory)
        {

            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                HandleCookies = true,
                BaseAddress = new Uri("https://localhost")
            });

            var loginRequest = new
            {
                email = "string2@g",
                password = "stringstring"
            };

            var response = await client.PostAsJsonAsync( "/api/Authentication/login",loginRequest);

            response.EnsureSuccessStatusCode();

            return client;

        }
       

    }
}
