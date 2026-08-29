namespace ClinicFlow.Application.Features.Patients.DTOs.Requests
{
    public sealed record PatientAppointmentSearchDtoRequest
    {
        public string Name { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
    }
}
