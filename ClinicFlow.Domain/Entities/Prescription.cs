namespace ClinicFlow.Domain.Entities
{
    public class Prescription
    {
        public int Id { get; set; }

        public int MedicalRecordId { get; set; }

        public int DoctorId { get; set; }

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }

        public MedicalRecord MedicalRecord { get; set; } = null!;

        public Doctor Doctor { get; set; } = null!;

        public ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
    }






}
