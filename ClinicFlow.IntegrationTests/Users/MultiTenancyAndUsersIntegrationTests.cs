using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ClinicFlow.IntegrationTests.Users
{
    public class MultiTenancyAndUsersIntegrationTests : IntegrationTestBase
    {
        public MultiTenancyAndUsersIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task GetUser_FromDifferentClinic_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);
            var response = await client.GetAsync("/api/Users/4"); // Clinic A Receptionist
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUser_WithoutManagePermission_ReturnsForbidden()
        {
            var client = await AuthenticationHelper.GetClinicADoctor1ClientAsync(Factory);
            var response = await client.GetAsync("/api/Users/4");
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetUser_Self_ReturnsSuccess()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var response = await client.GetAsync("/api/Users/1"); // Owner A is UserId 2
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ToggleUserStatus_FromDifferentClinic_ReturnsBadRequest()
        {
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);
            var response = await client.PutAsync("/api/Users/ToggleUserStatus?userId=4", null);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateReceptionist_WithValidData_ReturnsSuccessAndIsolatesToTenant()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var request = new CreateAndEditUserDtoRequest
            {
                FirstName = "New",
                LastName = "Receptionist",
                Email = "newrec2@g.com",
                Password = "stringstring",
                PhoneNumber = "11111111118" 
            };

            var response = await client.PostAsJsonAsync("/api/Users/CreateReceptionists", request);
            
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());
            var createdUserId = root.GetProperty("data").GetInt32();

            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlow.Infrastructure.Data.AppDbContext>();
            var createdUser = await dbContext.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == createdUserId);
                
            Assert.NotNull(createdUser);
            Assert.Equal(1, createdUser.ClinicId);
            Assert.Contains(createdUser.UserRoles, ur => ur.RoleId == 4);
        }

        [Fact]
        public async Task Me_ReturnsCurrentUserInformation()
        {
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);
            var response = await client.GetAsync("/api/Users/me");
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            
            var data = root.GetProperty("data");
            Assert.Equal("ahmed@g", data.GetProperty("email").GetString());
            Assert.Equal(2, data.GetProperty("clinicId").GetInt32());
            Assert.True(data.TryGetProperty("firstName", out _));
            Assert.True(data.TryGetProperty("lastName", out _));
            Assert.True(data.TryGetProperty("phoneNumber", out _));
        }

        [Fact]
        public async Task UpdateMyInformation_WhenUnauthenticated_ReturnsUnauthorized()
        {
            var client = Factory.CreateClient();
            var request = new UpdateMyInformationDtoRequest
            {
                FirstName = "NoAuth",
                LastName = "User",
                Email = "noauth@example.com",
                PhoneNumber = "01011223344"
            };

            var response = await client.PutAsJsonAsync("/api/Users/me", request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateMyInformation_WhenWhitespaceFields_ReturnsBadRequest()
        {
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);
            var request = new UpdateMyInformationDtoRequest
            {
                FirstName = "   ",
                LastName = "",
                Email = "not-an-email",
                PhoneNumber = "123"
            };

            var response = await client.PutAsJsonAsync("/api/Users/me", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateMyInformation_WhenEmailExistsForOtherPerson_ReturnsConflict()
        {
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);
            var request = new UpdateMyInformationDtoRequest
            {
                FirstName = "Ahmed",
                LastName = "Owner",
                Email = TestCredentials.ClinicAOwnerEmail, // already taken by Clinic A Owner
                PhoneNumber = "01099887766"
            };

            var response = await client.PutAsJsonAsync("/api/Users/me", request);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task UpdateMyInformation_WhenValidData_ReturnsSuccessAndUpdatesDatabase()
        {
            var client = await AuthenticationHelper.GetClinicAReceptionistClientAsync(Factory);
            var request = new UpdateMyInformationDtoRequest
            {
                FirstName = "UpdatedReceptionist",
                LastName = "UpdatedLastName",
                Email = "receptionist_new@g.com",
                PhoneNumber = "01234567890"
            };

            var response = await client.PutAsJsonAsync("/api/Users/me", request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlow.Infrastructure.Data.AppDbContext>();
            var receptionist = await dbContext.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Person.Email == "receptionist_new@g.com");

            Assert.NotNull(receptionist);
            Assert.Equal("UpdatedReceptionist", receptionist.Person.FirstName);
            Assert.Equal("UpdatedLastName", receptionist.Person.LastName);
            Assert.Equal("01234567890", receptionist.Person.PhoneNumber);
        }
    }
}

