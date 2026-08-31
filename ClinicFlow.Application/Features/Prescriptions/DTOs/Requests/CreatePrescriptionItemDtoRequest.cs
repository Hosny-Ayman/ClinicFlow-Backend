namespace ClinicFlow.Application.Features.Prescriptions.DTOs.Requests
{
    public sealed record CreatePrescriptionItemDtoRequest
    {
        public string MedicationName { get; init; } = null!;
        public string Dosage { get; init; } = null!;
        public string Frequency { get; init; } = null!;
        public string Duration { get; init; } = null!;
        public string? Instructions { get; init; }
    }
}
