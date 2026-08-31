namespace ClinicFlow.Application.Features.MedicalRecords.DTOs.Responses
{
    public sealed record GetMedicalRecordDtoResponse
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string? Symptoms { get; set; }
        public string TreatmentPlan { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
