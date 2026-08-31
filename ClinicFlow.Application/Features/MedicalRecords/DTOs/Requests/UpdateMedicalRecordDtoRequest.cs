namespace ClinicFlow.Application.Features.MedicalRecords.DTOs.Requests
{
    public sealed record UpdateMedicalRecordDtoRequest
    {
        public int Id { get; init; }
        public string Diagnosis { get; init; } = null!;
        public string? Symptoms { get; init; }
        public string TreatmentPlan { get; init; } = null!;
        public string? Notes { get; init; }
    }
}
