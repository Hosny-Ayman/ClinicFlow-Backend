namespace ClinicFlow.Application.Features.Clinics.DTOs.Responses
{
    public sealed record  CreateClinicResponse
    {

        public int ClinicId { get; init; }

        public string ClinicName { get; init; } = null!;

        public string ClinicEmail { get; init; } = null!;

        public string ClinicPhone { get; init; } = null!;

        public string ClinicAddress { get; init; } = null!;

        public string OwnerFullName { get; init; } = null!;

        public DateTime CreatedAt { get; init; }


    }
}
