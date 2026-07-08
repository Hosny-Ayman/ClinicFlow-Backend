using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Domain.Entities
{
    public class Patient
    {

        public int Id { get; set; }

        public int ClinicId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateOnly DateOfBirth { get; set; }

        public GenderEnum Gender { get; set; }

        public string PhoneNumber { get; set; } = null!;

        public string? Email { get; set; }

        public string? Notes { get; set; }

        public string? Address { get; set; }

        public BloodTypeEnum? BloodType { get; set; }

        public string? NationalId { get; set; }

        public string? EmergencyContactName { get; set; }

        public string? EmergencyContactPhone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Clinic Clinic { get; set; } = null!;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }












}
