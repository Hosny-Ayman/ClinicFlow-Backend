using ClinicFlow.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace ClinicFlow.IntegrationTests.Doctors
{
    public class DoctorIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public DoctorIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetDocto_WithAuthenticatedUser_Returns200()
        {
            // Arrange
            var client = await AuthenticationHelper
                .CreateAuthenticatedClientAsync(_factory);

            // Act
            var response = await client.GetAsync("/api/Doctors/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}