namespace ClinicFlow.Application.Features.Appointments.DTOs.Responses
{
    public sealed record GetAppointmentDashboardDtoResponse
    {

        public int TotalAppointments { get; set; }

        public int TodayAppointments { get; set; }

        public int PendingAppointments { get; set; }

        public int CancelledAppointments { get; set; }

    }
}
