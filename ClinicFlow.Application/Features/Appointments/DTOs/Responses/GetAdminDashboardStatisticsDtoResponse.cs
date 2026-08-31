namespace ClinicFlow.Application.Features.Appointments.DTOs.Responses
{
    public sealed record GetAdminDashboardStatisticsDtoResponse
    {
        public int TotalAppointments { get; set; }
        public int TotalAppointmentsDiff { get; set; } // جديد

        public int AttendedAppointments { get; set; }
        public int AttendedAppointmentsDiff { get; set; } // جديد

        public int WaitingAppointments { get; set; }
        public int WaitingAppointmentsDiff { get; set; } // جديد

        public int CancelledAppointments { get; set; }
        public int CancelledAppointmentsDiff { get; set; } // جديد

        public List<AppointmentStatusBreakdownDto> AppointmentsByStatus { get; set; } = new();
        public List<AppointmentTimeBreakdownDto> AppointmentsByTimePeriod { get; set; } = new();
        public List<TopDoctorDto> TopDoctors { get; set; } = new();
    }

    public sealed record AppointmentStatusBreakdownDto(
        string Status,
        int Count
    );

    public sealed record AppointmentTimeBreakdownDto(
        string TimePeriod,
        int Count
    );

    public sealed class TopDoctorDto
    {
        public string DoctorName { get; set; } = null!;
        public string Specialty { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int AppointmentCount { get; set; }
    }
}