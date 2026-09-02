using ClinicFlow.Application.Features.Patients.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ClinicFlow.IntegrationTests.Patients
{
    public class PatientIntegrationTests : IntegrationTestBase
    {
        public PatientIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreatePatient_WithValidData_ReturnsSuccessAndUpdatesDatabase()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);

            var request = new CreatePatientDtoRequest
            {
                FirstName = "New",
                LastName = "Patient",
                Email = "newpatient@g.com",
                PhoneNumber = "11111111111",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Gender = GenderEnum.Male,
                Notes = "Test Notes"
            };

            var response = await client.PostAsJsonAsync("/api/Patients", request);

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());
            
            var patientId = root.GetProperty("data").GetInt32();
            Assert.True(patientId > 0);

            // Verify Database State
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlow.Infrastructure.Data.AppDbContext>();

            var createdPatient = await dbContext.Patients.Include(p => p.Person).FirstOrDefaultAsync(p => p.Id == patientId);
            Assert.NotNull(createdPatient);
            Assert.Equal("New", createdPatient.Person.FirstName);
            
            var clinicPatient = await dbContext.ClinicPatients.FirstOrDefaultAsync(cp => cp.PatientId == patientId && cp.ClinicId == 1);
            Assert.NotNull(clinicPatient);
        }

        [Fact]
        public async Task CreatePatient_Unauthenticated_ReturnsUnauthorized()
        {
            var client = Factory.CreateClient();

            var request = new CreatePatientDtoRequest
            {
                FirstName = "New",
                LastName = "Patient",
                PhoneNumber = "11111111111",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Gender = GenderEnum.Male
            };

            var response = await client.PostAsJsonAsync("/api/Patients", request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreatePatient_WithInvalidValidationData_ReturnsBadRequest()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);

            var request = new CreatePatientDtoRequest
            {
                // Missing FirstName and LastName to trigger validation errors
                PhoneNumber = "123", // Invalid length
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), // Future date
                Gender = GenderEnum.Male
            };

            var response = await client.PostAsJsonAsync("/api/Patients", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetPatient_WithValidId_Returns200AndData()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);

            var response = await client.GetAsync("/api/Patients/1");

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());
            
            var data = root.GetProperty("data");
            Assert.Equal("Patient", data.GetProperty("firstName").GetString());
            Assert.Equal("A", data.GetProperty("lastName").GetString());
            Assert.Equal("Male", data.GetProperty("gender").GetString()); // Service maps enum to string
        }

        [Fact]
        public async Task GetPatient_FromDifferentClinic_ReturnsNotFound()
        {
            // Clinic B Owner trying to get Patient 1 (who belongs to Clinic A)
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);

            var response = await client.GetAsync("/api/Patients/1");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePatient_WithValidData_ReturnsSuccessAndUpdatesDatabase()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);

            var request = new UpdatePatientDtoRequest
            {
                Id = 1,
                FirstName = "UpdatedFirst",
                LastName = "UpdatedLast",
                PhoneNumber = "99999999999",
                DateOfBirth = new DateOnly(1985, 5, 5),
                Gender = GenderEnum.Male,
                Email = "updated@g.com"
            };

            var response = await client.PutAsJsonAsync("/api/Patients", request);

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            // Verify Database State
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlow.Infrastructure.Data.AppDbContext>();

            var updatedPatient = await dbContext.Patients.Include(p => p.Person).FirstOrDefaultAsync(p => p.Id == 1);
            Assert.NotNull(updatedPatient);
            Assert.Equal("UpdatedFirst", updatedPatient.Person.FirstName);
            Assert.Equal("UpdatedLast", updatedPatient.Person.LastName);
            Assert.Equal("99999999999", updatedPatient.Person.PhoneNumber);
            Assert.Equal(new DateOnly(1985, 5, 5), updatedPatient.DateOfBirth);
        }
    }
}
