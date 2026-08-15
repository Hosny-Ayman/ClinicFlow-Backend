namespace ClinicFlow.Application.Features.Patients.DTOs.Responses
{
    public sealed record GetAllPatientsDtoResponse
    {

        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; } = string.Empty;

        public string? NationalId { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }

        public string? BloodType { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

    }
}
