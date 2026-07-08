using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public AppointmentStatusEnum Status { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; } = null!;

        public Doctor Doctor { get; set; } = null!;

        public Invoice? Invoice { get; set; }

        public MedicalRecord? MedicalRecord { get; set; }
    }


}
