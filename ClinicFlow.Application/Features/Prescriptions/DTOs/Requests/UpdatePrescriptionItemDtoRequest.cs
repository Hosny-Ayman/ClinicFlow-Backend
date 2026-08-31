namespace ClinicFlow.Application.Features.Prescriptions.DTOs.Requests
{
    public sealed record UpdatePrescriptionItemDtoRequest
    {
        public int Id { get; init; }
        public string MedicationName { get; init; } = null!;
        public string Dosage { get; init; } = null!;
        public string Frequency { get; init; } = null!;
        public string Duration { get; init; } = null!;
        public string? Instructions { get; init; }
    }
}
