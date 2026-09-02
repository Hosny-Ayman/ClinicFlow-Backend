using ClinicFlow.Application.Features.Clinics.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ClinicFlow.IntegrationTests.Clinics
{
    public class ClinicIntegrationTests : IntegrationTestBase
    {
        public ClinicIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateClinic_WithValidData_ReturnsSuccessAndUpdatesDatabase()
        {
            var client = Factory.CreateClient();

            var content = new MultipartFormDataContent();
            content.Add(new StringContent("New Integration Clinic"), "Clinic.Name");
            content.Add(new StringContent("11111111111"), "Clinic.Phone");
            content.Add(new StringContent("newclinic@g.com"), "Clinic.Email");
            content.Add(new StringContent("123 Test St"), "Clinic.Address");
            content.Add(new StringContent("John"), "User.FirstName");
            content.Add(new StringContent("Doe"), "User.LastName");
            content.Add(new StringContent("johndoe@g.com"), "User.Email");
            content.Add(new StringContent("stringstring"), "User.Password");
            content.Add(new StringContent("11111111112"), "User.PhoneNumber");

            var response = await client.PostAsync("/api/Clinics", content);

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());
            
            var data = root.GetProperty("data");
            var clinicId = data.GetProperty("clinicId").GetInt32();
            Assert.Equal("New Integration Clinic", data.GetProperty("clinicName").GetString());

            // Verify Database State
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlow.Infrastructure.Data.AppDbContext>();

            var createdClinic = await dbContext.Clinics.FirstOrDefaultAsync(c => c.Id == clinicId);
            Assert.NotNull(createdClinic);
            Assert.Equal("New Integration Clinic", createdClinic.Name);

            var createdUser = await dbContext.Users.Include(u => u.Person).FirstOrDefaultAsync(u => u.Person.Email == "johndoe@g.com");
            Assert.NotNull(createdUser);
            Assert.Equal(clinicId, createdUser.ClinicId);
        }

        [Fact]
        public async Task CreateClinic_WithExistingUserEmail_ReturnsBadRequest()
        {
            var client = Factory.CreateClient();

            var content = new MultipartFormDataContent();
            content.Add(new StringContent("Another Clinic"), "Clinic.Name");
            content.Add(new StringContent("11111111113"), "Clinic.Phone");
            content.Add(new StringContent("another@g.com"), "Clinic.Email");
            content.Add(new StringContent("123 Test St"), "Clinic.Address");
            content.Add(new StringContent("John"), "User.FirstName");
            content.Add(new StringContent("Doe"), "User.LastName");
            // Use existing email from TestCredentials to trigger business logic failure
            content.Add(new StringContent(TestCredentials.SuperAdminEmail), "User.Email");
            content.Add(new StringContent("stringstring"), "User.Password");
            content.Add(new StringContent("11111111112"), "User.PhoneNumber");

            var response = await client.PostAsync("/api/Clinics", content);

            // Our ClinicService returns OperationResult.BadRequest() when user creation fails due to existing email
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.False(root.GetProperty("isSuccess").GetBoolean());
        }

        [Fact]
        public async Task CreateClinic_WithInvalidValidationData_ReturnsBadRequest()
        {
            var client = Factory.CreateClient();

            var content = new MultipartFormDataContent();
            // Missing Clinic.Name and Clinic.Phone
            content.Add(new StringContent("newclinic@g.com"), "Clinic.Email");
            
            var response = await client.PostAsync("/api/Clinics", content);

            // FluentValidation intercepts and returns 400
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetClinic_WithAuthorizedUser_Returns200AndMatchesSeededData()
        {
            // Owner A belongs to Clinic 1
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);

            var response = await client.GetAsync("/api/Clinics");

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());
            
            var data = root.GetProperty("data");
            Assert.Equal("Clinic A", data.GetProperty("name").GetString());
            Assert.Equal("clinica@g", data.GetProperty("email").GetString());
        }

        [Fact]
        public async Task GetClinic_Unauthenticated_ReturnsUnauthorized()
        {
            var client = Factory.CreateClient();

            var response = await client.GetAsync("/api/Clinics");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateClinic_AsReceptionist_ReturnsForbidden()
        {
            // Receptionist lacks ClinicsUpdate permission
            var client = await AuthenticationHelper.GetClinicAReceptionistClientAsync(Factory);

            var content = new MultipartFormDataContent();
            content.Add(new StringContent("Updated Name"), "Name");
            content.Add(new StringContent("11111111114"), "Phone");
            content.Add(new StringContent("clinica@g"), "Email");
            content.Add(new StringContent("Address A"), "Address");

            var response = await client.PutAsync("/api/Clinics", content);

            // Middleware denies access because Receptionist lacks the ClinicsUpdate policy
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}


