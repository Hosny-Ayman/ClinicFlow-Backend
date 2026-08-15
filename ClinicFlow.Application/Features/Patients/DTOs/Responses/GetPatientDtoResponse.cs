using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.Patients.DTOs.Responses
{
    public sealed record GetPatientDtoResponse
    {

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; } = null!;
        public string? Notes { get; set; }
        public string? Address { get; set; }
        public string? BloodType { get; set; }
        public string? NationalId { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
