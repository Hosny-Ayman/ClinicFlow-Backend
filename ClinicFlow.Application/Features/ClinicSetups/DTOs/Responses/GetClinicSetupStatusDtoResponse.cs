namespace ClinicFlow.Application.Features.ClinicSetups.DTOs.Responses
{
    public sealed record GetClinicSetupStatusDtoResponse
    {
       

        public bool IsSetupCompleted { get; init; }

        public bool HasSkippedSetup { get; init; }

        public double Progress { get; init; }

        public List<SetupStepDtoRequest> Steps { get; init; } = [];


    }
}
