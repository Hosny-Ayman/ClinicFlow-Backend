using ClinicFlow.Application.Features.Appointments.DTOs;
using ClinicFlow.Application.Features.Appointments.DTOs.Requests;
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

namespace ClinicFlow.IntegrationTests.Appointments
{
    public class AppointmentIntegrationTests : IntegrationTestBase
    {
        public AppointmentIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

        // Test Unauthenticated Create
        [Fact]
        public async Task Create_WithUnauthenticated_ReturnsUnauthorized()
        {
            var client = Factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
            var request = new CreateAndEditAppointmentDto
            {
                PatientId = 1,
                DoctorId = 2,
                AppointmentDate = new DateOnly(2030, 2, 4), // Monday
                StartTime = new TimeOnly(12, 0),
                EndTime = new TimeOnly(12, 15),
                Status = AppointmentStatusEnum.Scheduled
            };
            var response = await client.PostAsJsonAsync("/api/Appointments/CreateAppointment", request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // Test Forbidden Create (Doctor lacking AppointmentsCreate permission)
        [Fact]
        public async Task Create_WithoutAppointmentsCreatePermission_ReturnsForbidden()
        {
            // Doctor 1 has RoleId=3 which does not have AppointmentsCreate permission
            var client = await AuthenticationHelper.GetClinicADoctor1ClientAsync(Factory);
            var request = new CreateAndEditAppointmentDto
            {
                PatientId = 1,
                DoctorId = 2,
                AppointmentDate = new DateOnly(2030, 2, 4),
                StartTime = new TimeOnly(12, 0),
                EndTime = new TimeOnly(12, 15),
                Status = AppointmentStatusEnum.Scheduled
            };
            var response = await client.PostAsJsonAsync("/api/Appointments/CreateAppointment", request);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        // Test Valid Create
        [Fact]
        public async Task Create_WithValidData_Returns200AndCreatesAppointment()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var request = new CreateAndEditAppointmentDto
            {
                PatientId = 1,
                DoctorId = 2, // Doctor A
                AppointmentDate = new DateOnly(2030, 2, 4), // Monday, future date
                StartTime = new TimeOnly(12, 0),
                EndTime = new TimeOnly(12, 15),
                Status = AppointmentStatusEnum.Scheduled,
                Notes = "Valid Test Appointment"
            };

            var response = await client.PostAsJsonAsync("/api/Appointments/CreateAppointment", request);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());
            var appointmentId = root.GetProperty("data").GetInt32();
            Assert.True(appointmentId > 0);

            // Verify in DB
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlow.Infrastructure.Data.AppDbContext>();
            var appointment = await dbContext.Appointments.FindAsync(appointmentId);
            Assert.NotNull(appointment);
            Assert.Equal(1, appointment.ClinicId);
            Assert.Equal("Valid Test Appointment", appointment.Notes);
        }

        // Test Invalid Create (Doctor Vacation)
        [Fact]
        public async Task Create_DuringDoctorVacation_ReturnsBadRequest()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var request = new CreateAndEditAppointmentDto
            {
                PatientId = 1,
                DoctorId = 2, // Doctor A
                AppointmentDate = new DateOnly(2026, 1, 5), // Doctor A has vacation 2026-01-01 to 2026-01-10
                StartTime = new TimeOnly(12, 0),
                EndTime = new TimeOnly(12, 15),
                Status = AppointmentStatusEnum.Scheduled
            };

            var response = await client.PostAsJsonAsync("/api/Appointments/CreateAppointment", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // Test Invalid Duration (Duration does not match Clinic.AppointmentDurationInMinutes)
        [Fact]
        public async Task Create_WithInvalidAppointmentDuration_ReturnsBadRequest()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var request = new CreateAndEditAppointmentDto
            {
                PatientId = 1,
                DoctorId = 2,
                AppointmentDate = new DateOnly(2030, 2, 4),
                StartTime = new TimeOnly(12, 0),
                EndTime = new TimeOnly(12, 30), // Clinic duration is 15 minutes, 30 is invalid
                Status = AppointmentStatusEnum.Scheduled
            };

            var response = await client.PostAsJsonAsync("/api/Appointments/CreateAppointment", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("15", body);
        }

        // Test Non-Existing Patient
        [Fact]
        public async Task Create_WithNonExistingPatient_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var request = new CreateAndEditAppointmentDto
            {
                PatientId = 9999, // Non-existent patient
                DoctorId = 2,
                AppointmentDate = new DateOnly(2030, 2, 4),
                StartTime = new TimeOnly(12, 0),
                EndTime = new TimeOnly(12, 15),
                Status = AppointmentStatusEnum.Scheduled
            };

            var response = await client.PostAsJsonAsync("/api/Appointments/CreateAppointment", request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Test Non-Existing Doctor
        [Fact]
        public async Task Create_WithNonExistingDoctor_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var request = new CreateAndEditAppointmentDto
            {
                PatientId = 1,
                DoctorId = 9999, // Non-existent doctor
                AppointmentDate = new DateOnly(2030, 2, 4),
                StartTime = new TimeOnly(12, 0),
                EndTime = new TimeOnly(12, 15),
                Status = AppointmentStatusEnum.Scheduled
            };

            var response = await client.PostAsJsonAsync("/api/Appointments/CreateAppointment", request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Test Cross-Clinic Creation
        [Fact]
        public async Task Create_ForDoctorInDifferentClinic_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory); // Caller is Clinic B
            var request = new CreateAndEditAppointmentDto
            {
                PatientId = 1, // Clinic A Patient
                DoctorId = 2, // Clinic A Doctor
                AppointmentDate = new DateOnly(2030, 2, 4),
                StartTime = new TimeOnly(12, 0),
                EndTime = new TimeOnly(12, 15),
                Status = AppointmentStatusEnum.Scheduled
            };

            var response = await client.PostAsJsonAsync("/api/Appointments/CreateAppointment", request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Test GetAllAppointment
        [Fact]
        public async Task GetAllAppointment_WithValidSearch_Returns200AndData()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var request = new AppointmentSearchDtoRequest
            {
                PageNumber = 1,
                PageSize = 10
            };

            var response = await client.PostAsJsonAsync("/api/Appointments/GetAllAppointment", request);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());

            var pagedData = root.GetProperty("data");
            var items = pagedData.GetProperty("data");
            Assert.Equal(JsonValueKind.Array, items.ValueKind);
            Assert.True(pagedData.GetProperty("totalRecords").GetInt32() >= 1);
        }

        // Test GetDoctorAvailableSlots
        [Fact]
        public async Task GetDoctorAvailableSlots_WithValidDoctor_ReturnsAvailableSlots()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            // 2030-01-07 is Monday, Clinic 1 is open, Doctor 2 has schedules 9:00 - 17:00
            var response = await client.GetAsync("/api/Appointments/GetDoctorAvailableSlots?doctorId=2&appointmentDate=2030-01-07");
            
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());
            
            var slots = root.GetProperty("data");
            Assert.Equal(JsonValueKind.Array, slots.ValueKind);
            Assert.True(slots.GetArrayLength() > 0);
        }

        // Test Update Status (Valid Transition: Scheduled -> Cancelled)
        [Fact]
        public async Task UpdateStatus_ValidTransition_Returns200AndUpdatesDB()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            // 1 is our seeded Scheduled appointment
            var response = await client.PutAsync("/api/Appointments/UpdateAppointmentStatus?appointmentId=1&status=5", null); // Cancelled = 5
            
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlow.Infrastructure.Data.AppDbContext>();
            var appointment = await dbContext.Appointments.FindAsync(1);
            Assert.Equal(AppointmentStatusEnum.Cancelled, appointment.Status);
        }

        // Test Update Status (Invalid Transition)
        [Fact]
        public async Task UpdateStatus_InvalidTransition_ReturnsBadRequest()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            // 1 is our seeded Scheduled appointment
            // Transition Scheduled -> Completed is not allowed
            var response = await client.PutAsync("/api/Appointments/UpdateAppointmentStatus?appointmentId=1&status=4", null); // Completed = 4
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // Test Update Status (Non-Existing Appointment)
        [Fact]
        public async Task UpdateStatus_WithNonExistingAppointment_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var response = await client.PutAsync("/api/Appointments/UpdateAppointmentStatus?appointmentId=9999&status=5", null);
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Test Update Status (Cross-Clinic)
        [Fact]
        public async Task UpdateStatus_CrossClinicAppointment_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);
            var response = await client.PutAsync("/api/Appointments/UpdateAppointmentStatus?appointmentId=1&status=5", null);
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
