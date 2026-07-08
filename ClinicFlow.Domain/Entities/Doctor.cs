using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Domain.Entities
{
    public class Doctor
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public int ClinicId { get; set; }

        public int SpecialtyId { get; set; }

        public decimal ConsultationFee { get; set; }

        public string Bio { get; set; } = null!;

        public GenderEnum Gender { get; set; }

        public int ExperienceYears { get; set; }

        public User User { get; set; } = null!;

        public Clinic Clinic { get; set; } = null!;

        public Specialty Specialty { get; set; } = null!;

        public string ProfileImageUrl { get; set; } = null!;

        public ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();

        public ICollection<DoctorVacation> DoctorVacations { get; set; } = new List<DoctorVacation>();

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    }
}
