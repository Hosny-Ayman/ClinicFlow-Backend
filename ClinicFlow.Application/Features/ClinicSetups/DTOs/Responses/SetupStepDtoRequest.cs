namespace ClinicFlow.Application.Features.ClinicSetups.DTOs.Responses
{
    public sealed record SetupStepDtoRequest
    {
        public string Key { get; init; } = null!;

        public string Title { get; init; } = null!;

        public bool IsCompleted { get; init; }

    }
}
