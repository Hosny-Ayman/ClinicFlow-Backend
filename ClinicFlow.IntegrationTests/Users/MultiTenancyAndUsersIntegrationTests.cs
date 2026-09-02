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
        }
    }
}

