namespace ClinicFlow.Application.Features.ClinicSetups.DTOs.Requests
{
    public sealed record CreateAndEditClinicSetupDtoRequest
    {
        public bool HasSkippedSetup { get; init; }
    }
}
