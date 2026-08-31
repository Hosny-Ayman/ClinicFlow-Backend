namespace ClinicFlow.Application.Features.Prescriptions.DTOs.Responses
{
    public sealed record GetPrescriptionItemDtoResponse
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? Instructions { get; set; }
    }
}
