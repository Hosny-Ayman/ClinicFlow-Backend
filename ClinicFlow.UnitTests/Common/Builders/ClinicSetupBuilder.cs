using ClinicFlow.Application.Features.ClinicSetups.DTOs.Requests;
using ClinicFlow.Application.Features.ClinicSetups.DTOs.Responses;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class ClinicSetupBuilder
    {
        public static CreateAndEditClinicSetupDtoRequest CreateAndEditClinicSetupDto(bool hasSkippedSetup = true)
        {
            return new CreateAndEditClinicSetupDtoRequest
            {
                HasSkippedSetup = hasSkippedSetup
            };
        }

        public static ClinicSetup CreateClinicSetupEntity(int id = 1, int clinicId = 10, bool hasSkippedSetup = false)
        {
            return new ClinicSetup
            {
                Id = id,
                ClinicId = clinicId,
                HasSkippedSetup = hasSkippedSetup
            };
        }

        public static GetClinicSetupStatusDtoResponse GetClinicSetupStatusDtoResponse()
        {
            return new GetClinicSetupStatusDtoResponse
            {
                IsSetupCompleted = false,
                HasSkippedSetup = false,
                Progress = 50.0,
                Steps = new List<SetupStepDtoRequest>
                {
                    new SetupStepDtoRequest { Key = "step1", Title = "Step 1", IsCompleted = true },
                    new SetupStepDtoRequest { Key = "step2", Title = "Step 2", IsCompleted = false }
                }
            };
        }
    }
}
