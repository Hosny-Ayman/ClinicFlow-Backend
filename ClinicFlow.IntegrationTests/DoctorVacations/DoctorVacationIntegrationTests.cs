using ClinicFlow.Application.Features.DoctorVacations.DTOs;
using ClinicFlow.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ClinicFlow.IntegrationTests.DoctorVacations
{
    public class DoctorVacationIntegrationTests : IntegrationTestBase
    {
        public DoctorVacationIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

        // Test Unauthenticated
        [Fact]
        public async Task Create_WithUnauthenticated_ReturnsUnauthorized()
        {
            var client = Factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
            var request = new Get_Create_Update_DoctorVacationDto { UserId = 8, StartDate = new DateOnly(2026, 12, 1), EndDate = new DateOnly(2026, 12, 10) };
            var response = await client.PostAsJsonAsync("/api/DoctorVacations", request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // Test Valid Create
        [Fact]
        public async Task Create_WithValidData_Returns200AndCreatesVacation()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var request = new Get_Create_Update_DoctorVacationDto
            {
                UserId = 8, // Doctor A
                StartDate = new DateOnly(2026, 12, 1),
                EndDate = new DateOnly(2026, 12, 10),
                Reason = "Family Vacation"
            };

            var response = await client.PostAsJsonAsync("/api/DoctorVacations", request);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());
            var vacationId = root.GetProperty("data").GetInt32();
            Assert.True(vacationId > 0);

            // DB Verify
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlow.Infrastructure.Data.AppDbContext>();
            var vacation = await dbContext.DoctorVacations.FindAsync(vacationId);
            Assert.NotNull(vacation);
            Assert.Equal(2, vacation.DoctorId); // Doctor A's DoctorId = 2
            Assert.Equal(request.Reason, vacation.Reason);
        }

        // Test Invalid Create (Validation)
        [Fact]
        public async Task Create_WithEndDateBeforeStartDate_ReturnsBadRequest()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var request = new Get_Create_Update_DoctorVacationDto
            {
                UserId = 8,
                StartDate = new DateOnly(2026, 12, 10),
                EndDate = new DateOnly(2026, 12, 1), // Invalid!
                Reason = "Invalid Vacation"
            };

            var response = await client.PostAsJsonAsync("/api/DoctorVacations", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("EndDate must be greater than or equal to StartDate", body);
        }

        // Test Tenant Isolation Create (Cross-Clinic Doctor)
        [Fact]
        public async Task Create_ForDoctorInDifferentClinic_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);
            var request = new Get_Create_Update_DoctorVacationDto
            {
                UserId = 8, // Doctor A (in Clinic 1)
                StartDate = new DateOnly(2026, 12, 1),
                EndDate = new DateOnly(2026, 12, 10)
            };

            var response = await client.PostAsJsonAsync("/api/DoctorVacations", request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Test Get (Valid)
        [Fact]
        public async Task Get_WithValidId_Returns200AndVacation()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var response = await client.GetAsync("/api/DoctorVacations/8/1"); // UserId=8, VacationId=1

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");
            
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());
            Assert.Equal(1, root.GetProperty("data").GetProperty("id").GetInt32());
            Assert.Equal("Test Vacation A", root.GetProperty("data").GetProperty("reason").GetString());
        }

        // Test Get (Cross-Clinic)
        [Fact]
        public async Task Get_CrossClinicVacation_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);
            // Doctor A is UserId 8, VacationId 1
            var response = await client.GetAsync("/api/DoctorVacations/8/1"); 
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Test Get (Non-Existing Vacation)
        [Fact]
        public async Task Get_NonExistingVacation_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var response = await client.GetAsync("/api/DoctorVacations/8/999"); 
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Test Update (Valid)
        [Fact]
        public async Task Update_WithValidData_Returns200AndUpdatesDatabase()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var request = new Get_Create_Update_DoctorVacationDto
            {
                Id = 1, // Updating Vacation 1
                UserId = 8,
                StartDate = new DateOnly(2026, 1, 1),
                EndDate = new DateOnly(2026, 1, 15),
                Reason = "Updated Reason"
            };

            var response = await client.PutAsJsonAsync("/api/DoctorVacations", request);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            // Verify DB
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlow.Infrastructure.Data.AppDbContext>();
            var vacation = await dbContext.DoctorVacations.FindAsync(1);
            Assert.Equal("Updated Reason", vacation.Reason);
            Assert.Equal(new DateOnly(2026, 1, 15), vacation.EndDate);
        }

        // Test Update (Cross-Clinic)
        [Fact]
        public async Task Update_CrossClinicVacation_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);
            var request = new Get_Create_Update_DoctorVacationDto
            {
                Id = 1,
                UserId = 8, // Doctor A
                StartDate = new DateOnly(2026, 1, 1),
                EndDate = new DateOnly(2026, 1, 15)
            };

            var response = await client.PutAsJsonAsync("/api/DoctorVacations", request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
