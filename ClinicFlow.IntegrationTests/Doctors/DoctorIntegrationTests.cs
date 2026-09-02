using ClinicFlow.IntegrationTests.Infrastructure;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ClinicFlow.IntegrationTests.Doctors
{
    public class DoctorIntegrationTests : IntegrationTestBase
    {
        public DoctorIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetDoctor_AsSuperAdmin_Returns200AndDataMatches()
        {
            var client = await AuthenticationHelper.GetSuperAdminClientAsync(Factory);

            var response = await client.GetAsync("/api/Doctors/1");

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Body: {responseBody}");

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.GetProperty("isSuccess").GetBoolean());

            var data = root.GetProperty("data");
            var user = data.GetProperty("user");
            var doctor = data.GetProperty("doctor");

            Assert.Equal(1, user.GetProperty("id").GetInt32());
            Assert.Equal("Owner", user.GetProperty("firstName").GetString());
            Assert.Equal("A", user.GetProperty("lastName").GetString());
            Assert.Equal(TestCredentials.ClinicAOwnerEmail, user.GetProperty("email").GetString());
            Assert.True(user.GetProperty("isActive").GetBoolean());

            Assert.Equal(1, doctor.GetProperty("id").GetInt32());
            Assert.Equal("Cardiology", doctor.GetProperty("specialtieName").GetString());
            Assert.Equal(300m, doctor.GetProperty("consultationFee").GetDecimal());
            Assert.Equal(5, doctor.GetProperty("experienceYears").GetInt32());
            
            // Note: Enum serialization might be int or string. The response DTO has string Gender so it should be a string.
            Assert.Equal("Male", doctor.GetProperty("gender").GetString());
        }

        [Fact]
        public async Task GetDoctor_Unauthenticated_ReturnsUnauthorized()
        {
            var client = Factory.CreateClient();

            var response = await client.GetAsync("/api/Doctors/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetDoctor_AsReceptionist_ReturnsForbidden()
        {
            // Receptionist lacks manage permissions for the doctor in EnsureCanManageUser
            var client = await AuthenticationHelper.GetClinicAReceptionistClientAsync(Factory);

            var response = await client.GetAsync("/api/Doctors/1");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetDoctor_NonExistingId_ReturnsNotFound()
        {
            var client = await AuthenticationHelper.GetSuperAdminClientAsync(Factory);

            var response = await client.GetAsync("/api/Doctors/999");

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected NotFound, got {response.StatusCode}. Body: {responseBody}");
            
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.False(root.GetProperty("isSuccess").GetBoolean());
            
            var errors = root.GetProperty("errors").EnumerateArray();
            var hasNotFoundMessage = false;
            foreach (var error in errors)
            {
                if (error.GetProperty("message").GetString() == "Doctor Not Found")
                {
                    hasNotFoundMessage = true;
                    break;
                }
            }
            Assert.True(hasNotFoundMessage, "Expected 'Doctor Not Found' error message.");
        }

        [Fact]
        public async Task GetDoctor_FromDifferentClinic_ReturnsNotFound()
        {
            
            var client = await AuthenticationHelper.GetClinicBOwnerClientAsync(Factory);

            var response = await client.GetAsync("/api/Doctors/1");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
