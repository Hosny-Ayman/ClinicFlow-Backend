namespace ClinicFlow.Application.Features.Appointments.DTOs.Responses
{
    public sealed record GetAllAppointmentDtoResponse
    {

        public int AppointmentId { get; set; }

        public TimeOnly Time { get; set; }

        public string PatientFullName { get; set; } = string.Empty;

        public string PatientPhoneNumber { get; set; } = string.Empty;

        public string DoctorFullName { get; set; } = string.Empty;

        public string DoctorSpecialtie { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public decimal ConsultationFee { get; set; }

        public string Paymentstatus { get; set; } = string.Empty;
    }
}
