namespace ClinicFlow.Application.Features.Appointments.DTOs
{
    public sealed record  SlotDto
    {
        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public SlotStatus Status { get; set; }

    }

    public enum SlotStatus
    {
        Booked =1,
        Available =2,
        Unavailable = 3

    }
}
