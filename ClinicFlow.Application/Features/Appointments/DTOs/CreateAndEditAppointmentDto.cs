using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.Appointments.DTOs
{
    public sealed record CreateAndEditAppointmentDto
    {

        public int? Id { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public AppointmentStatusEnum Status { get; set; }

        public string? Notes { get; set; }

    }
}
