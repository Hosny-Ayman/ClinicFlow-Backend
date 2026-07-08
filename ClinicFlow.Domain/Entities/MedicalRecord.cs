using System.Net.Mail;

namespace ClinicFlow.Domain.Entities
{
    public class MedicalRecord
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public int AppointmentId { get; set; }

        public string Diagnosis { get; set; } = null!;

        public string? Symptoms { get; set; }

        public string TreatmentPlan { get; set; } = null!;

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; } = null!;

        public Doctor Doctor { get; set; } = null!;

        public Appointment Appointment { get; set; } = null!;

        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }









}
