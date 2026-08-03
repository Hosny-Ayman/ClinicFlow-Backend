namespace ClinicFlow.Application.Features.ClinicSetups.DTOs.Requests
{
    public sealed record CreateClinicSetupDtoRequest
    {

        public bool IsSetupCompleted { get; init; }

        public bool HasSkippedSetup { get; init; }


    }
}
