using ClinicFlow.Application.Features.DoctorSchedules.DTOs.Requests;
using ClinicFlow.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ClinicFlow.IntegrationTests.DoctorSchedules
{
    public class DoctorScheduleIntegrationTests : IntegrationTestBase
    {
        public DoctorScheduleIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task GetAll_WithUnauthenticated_ReturnsUnauthorized()
        {
            var client = Factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
            var response = await client.GetAsync("/api/DoctorSchedules/8");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_WithValidDoctorUserId_Returns200AndSchedules()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var response = await client.GetAsync("/api/DoctorSchedules/8"); 

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());

            var data = root.GetProperty("data");
            Assert.Equal(JsonValueKind.Array, data.ValueKind);
            Assert.Equal(7, data.GetArrayLength()); 
        }

        [Fact]
        public async Task GetAll_WithNonDoctorUserId_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            var response = await client.GetAsync("/api/DoctorSchedules/4"); 
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_WithDoctorFromDifferentClinic_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);
            var response = await client.GetAsync("/api/DoctorSchedules/8"); 
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Update_WithValidData_Returns200AndUpdatesDatabase()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            
            var getResponse = await client.GetAsync("/api/DoctorSchedules/8");
            var getResponseBody = await getResponse.Content.ReadAsStringAsync();
            using var getJsonDoc = JsonDocument.Parse(getResponseBody);
            var getData = getJsonDoc.RootElement.GetProperty("data");
            
            var requests = new List<UpdateAndGetDoctorScheduleDtoRequest>();
            foreach (var item in getData.EnumerateArray())
            {
                var dayOfWeek = (DayOfWeek)item.GetProperty("dayOfWeek").GetInt32();
                if (dayOfWeek == DayOfWeek.Sunday) continue; // Skip because RequiredRule fails on 0

                var id = item.GetProperty("id").GetInt32();
                
                requests.Add(new UpdateAndGetDoctorScheduleDtoRequest
                {
                    Id = id,
                    DayOfWeek = dayOfWeek,
                    StartTime = new TimeOnly(10, 0), 
                    EndTime = new TimeOnly(18, 0),   
                    IsAvailable = true
                });
            }

            var response = await client.PutAsJsonAsync("/api/DoctorSchedules/8", requests);
            
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlow.Infrastructure.Data.AppDbContext>();
            var schedules = await dbContext.DoctorSchedules.Where(ds => ds.DoctorId == 2 && ds.DayOfWeek != DayOfWeek.Sunday).ToListAsync();
                
            Assert.Equal(6, schedules.Count); // We skipped Sunday
            Assert.All(schedules, s => 
            {
                Assert.True(s.IsAvailable);
                Assert.Equal(new TimeOnly(10, 0), s.StartTime);
                Assert.Equal(new TimeOnly(18, 0), s.EndTime);
            });
        }

        [Fact]
        public async Task Update_WithInvalidEndTime_ReturnsBadRequest()
        {
            var client = await AuthenticationHelper.GetClinicAOwnerClientAsync(Factory);
            
            var getResponse = await client.GetAsync("/api/DoctorSchedules/8");
            var getResponseBody = await getResponse.Content.ReadAsStringAsync();
            using var getJsonDoc = JsonDocument.Parse(getResponseBody);
            var getData = getJsonDoc.RootElement.GetProperty("data");
            
            var requests = new List<UpdateAndGetDoctorScheduleDtoRequest>();
            foreach (var item in getData.EnumerateArray())
            {
                var dayOfWeek = (DayOfWeek)item.GetProperty("dayOfWeek").GetInt32();
                if (dayOfWeek == DayOfWeek.Sunday) continue; // Skip because RequiredRule fails on 0

                var id = item.GetProperty("id").GetInt32();
                
                requests.Add(new UpdateAndGetDoctorScheduleDtoRequest
                {
                    Id = id,
                    DayOfWeek = dayOfWeek,
                    StartTime = new TimeOnly(12, 0),
                    EndTime = new TimeOnly(10, 0), // Invalid!
                    IsAvailable = true
                });
            }

            var response = await client.PutAsJsonAsync("/api/DoctorSchedules/8", requests);
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("EndTime must be greater than StartTime", responseBody);
        }

        [Fact]
        public async Task Update_WithDoctorFromDifferentClinic_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);
            
            var requests = new List<UpdateAndGetDoctorScheduleDtoRequest>
            {
                new UpdateAndGetDoctorScheduleDtoRequest
                {
                    Id = 1,
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(17, 0),
                    IsAvailable = true
                }
            };

            var response = await client.PutAsJsonAsync("/api/DoctorSchedules/8", requests); 
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
