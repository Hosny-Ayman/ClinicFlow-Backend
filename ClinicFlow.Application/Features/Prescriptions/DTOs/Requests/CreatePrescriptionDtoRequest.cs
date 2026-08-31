namespace ClinicFlow.Application.Features.Prescriptions.DTOs.Requests
{
    public sealed record CreatePrescriptionDtoRequest
    {
        public int MedicalRecordId { get; init; }
        public string? Notes { get; init; }
        public List<CreatePrescriptionItemDtoRequest> PrescriptionItems { get; init; } = new();
    }
}
