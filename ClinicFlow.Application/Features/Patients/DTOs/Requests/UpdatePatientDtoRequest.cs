using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.Patients.DTOs.Requests
{
    public sealed record UpdatePatientDtoRequest
    {

        public int Id { get; init; }
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public DateOnly DateOfBirth { get; init; }
        public GenderEnum Gender { get; init; }
        public string? Notes { get; init; }
        public string? Address { get; init; }
        public BloodTypeEnum? BloodType { get; init; }
        public string? NationalId { get; init; }
        public string? EmergencyContactName { get; init; }
        public string? EmergencyContactPhone { get; init; }

    }
}
