namespace ClinicFlow.Domain.Entities
{
    public class ClinicPatient
    {
        public int Id { get; set; }

        public int ClinicId { get; set; }

        public int PatientId { get; set; }

        public DateTime? FirstVisitDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Clinic Clinic { get; set; } = null!;

        public Patient Patient { get; set; } = null!;
    }
}
