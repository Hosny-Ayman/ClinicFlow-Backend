namespace ClinicFlow.Application.Features.Prescriptions.DTOs.Responses
{
    public sealed record GetPrescriptionDtoResponse
    {
        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public int DoctorId { get; set; }
        public DateTime IssuedAt { get; set; }
        public string? Notes { get; set; }
        public List<GetPrescriptionItemDtoResponse> PrescriptionItems { get; set; } = new();
    }
}
