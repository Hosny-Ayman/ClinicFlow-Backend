namespace ClinicFlow.Application.Features.Patients.DTOs
{
    public sealed record NotificationAppointmentPatientInfoDto
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentTime { get; set; }



    }
}
