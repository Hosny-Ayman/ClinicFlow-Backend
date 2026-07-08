namespace ClinicFlow.Domain.Entities
{
    public class PrescriptionItem
    {
        public int Id { get; set; }

        public int PrescriptionId { get; set; }

        public string MedicationName { get; set; } = null!;

        public string Dosage { get; set; } = null!;

        public string Frequency { get; set; } = null!;

        public string Duration { get; set; } = null!;

        public string? Instructions { get; set; }

        public Prescription Prescription { get; set; } = null!;
    }



}
