using ClinicFlow.Domain.Enums;
using ClinicFlow.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ClinicFlow.IntegrationTests.Doctors
{
    public class DoctorCreateIntegrationTests : IntegrationTestBase
    {
        public DoctorCreateIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateDoctor_WithValidData_ReturnsSuccessAndUpdatesDatabase()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);

            var content = new MultipartFormDataContent();
            // Doctor fields
            content.Add(new StringContent("3"), "Doctor.SpecialtyId");
            content.Add(new StringContent("500"), "Doctor.ConsultationFee");
            content.Add(new StringContent("1"), "Doctor.Gender"); // 1 = Male
            content.Add(new StringContent("10"), "Doctor.ExperienceYears");
            content.Add(new StringContent("Expert Cardiologist"), "Doctor.Bio");

            // User fields
            content.Add(new StringContent("New"), "User.FirstName");
            content.Add(new StringContent("Doctor"), "User.LastName");
            content.Add(new StringContent("newdoc@g.com"), "User.Email");
            content.Add(new StringContent("stringstring"), "User.Password");
            content.Add(new StringContent("11111111115"), "User.PhoneNumber"); // 11 digits

            var response = await client.PostAsync("/api/Doctors", content);

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());
            
            var doctorId = root.GetProperty("data").GetInt32();
            Assert.True(doctorId > 0);

            // Verify DB State
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlow.Infrastructure.Data.AppDbContext>();

            var createdDoctor = await dbContext.Doctors
                .Include(d => d.User).ThenInclude(u => u.Person)
                .Include(d => d.User).ThenInclude(u => u.UserRoles)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            Assert.NotNull(createdDoctor);
            Assert.Equal(1, createdDoctor.ClinicId); // Because ClinicAOwner is ClinicId=1
            Assert.Equal("newdoc@g.com", createdDoctor.User.Person.Email);
            Assert.Contains(createdDoctor.User.UserRoles, ur => ur.RoleId == 3); // 3 = Doctor Role
        }

        [Fact]
        public async Task CreateDoctor_WithExistingUserEmail_ReturnsConflict()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);

            var content = new MultipartFormDataContent();
            content.Add(new StringContent("3"), "Doctor.SpecialtyId");
            content.Add(new StringContent("500"), "Doctor.ConsultationFee");
            content.Add(new StringContent("1"), "Doctor.Gender"); 
            content.Add(new StringContent("10"), "Doctor.ExperienceYears");

            content.Add(new StringContent("New"), "User.FirstName");
            content.Add(new StringContent("Doctor"), "User.LastName");
            // Use SuperAdmin's email to trigger Conflict
            content.Add(new StringContent(TestCredentials.SuperAdminEmail), "User.Email");
            content.Add(new StringContent("stringstring"), "User.Password");
            content.Add(new StringContent("11111111116"), "User.PhoneNumber"); 

            var response = await client.PostAsync("/api/Doctors", content);

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.Conflict, $"Expected Conflict, got {response.StatusCode}. Body: {responseBody}");
            
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.False(root.GetProperty("isSuccess").GetBoolean());
        }

        [Fact]
        public async Task CreateDoctor_Unauthenticated_ReturnsUnauthorized()
        {
            var client = Factory.CreateClient();

            var content = new MultipartFormDataContent();
            content.Add(new StringContent("3"), "Doctor.SpecialtyId");

            var response = await client.PostAsync("/api/Doctors", content);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateDoctor_WithInvalidValidationData_ReturnsBadRequest()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);

            var content = new MultipartFormDataContent();
            // Missing Doctor properties completely, just putting User
            content.Add(new StringContent("New"), "User.FirstName");
            content.Add(new StringContent("Doctor"), "User.LastName");
            content.Add(new StringContent("badvalidation@g.com"), "User.Email");
            content.Add(new StringContent("stringstring"), "User.Password");
            content.Add(new StringContent("123"), "User.PhoneNumber"); // Invalid phone length

            var response = await client.PostAsync("/api/Doctors", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateDoctorSteps_WhenUserAlreadyDoctor_ReturnsConflict()
        {
            // Clinic Owner A is already mapped to RoleId=3 (Doctor)
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);

            var content = new MultipartFormDataContent();
            content.Add(new StringContent("3"), "SpecialtyId");
            content.Add(new StringContent("500"), "ConsultationFee");
            content.Add(new StringContent("1"), "Gender"); 
            content.Add(new StringContent("10"), "ExperienceYears");

            var response = await client.PostAsync("/api/Doctors/steps", content);

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.Conflict, $"Expected Conflict, got {response.StatusCode}. Body: {responseBody}");
        }

        [Fact]
        public async Task CreateDoctorSteps_WithValidData_ReturnsSuccessAndPromotesUserToDoctor()
        {
            // Clinic Owner B is NOT mapped to RoleId=3 (Doctor) in Seeder, only RoleId=2
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);

            var content = new MultipartFormDataContent();
            content.Add(new StringContent("1"), "SpecialtyId"); // 1 = General Practice
            content.Add(new StringContent("300"), "ConsultationFee");
            content.Add(new StringContent("1"), "Gender"); 
            content.Add(new StringContent("12"), "ExperienceYears");
            content.Add(new StringContent("Owner B Bio"), "Bio");

            var response = await client.PostAsync("/api/Doctors/steps", content);

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());
            
            var doctorId = root.GetProperty("data").GetInt32();
            Assert.True(doctorId > 0);

            // Verify DB State
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlow.Infrastructure.Data.AppDbContext>();

            var createdDoctor = await dbContext.Doctors
                .Include(d => d.User).ThenInclude(u => u.UserRoles)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            Assert.NotNull(createdDoctor);
            Assert.Equal(2, createdDoctor.ClinicId); // Because ClinicBOwner is ClinicId=2
            Assert.Equal(5, createdDoctor.UserId); // Owner B UserId is 5
            Assert.Contains(createdDoctor.User.UserRoles, ur => ur.RoleId == 3); // Now has Doctor Role
        }
    }
}
