namespace ClinicFlow.Application.Features.Patients.DTOs.Responses
{
    public sealed record GetPatientInformationForAppointmentDtoResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
       
    }
}
