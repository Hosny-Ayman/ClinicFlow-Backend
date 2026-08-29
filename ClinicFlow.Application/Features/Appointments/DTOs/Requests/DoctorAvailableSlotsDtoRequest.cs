namespace ClinicFlow.Application.Features.Appointments.DTOs.Requests
{
    public sealed record DoctorAvailableSlotsDtoRequest
    {
        public int doctorId { get; init; }  
        public DateOnly appointmentDate { get; init; }

    }
}
