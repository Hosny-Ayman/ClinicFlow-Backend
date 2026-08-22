using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.DoctorVacations.DTOs.Responses
{
    public sealed record GetAllDoctorVacationInformationDtoResponse
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string? Reason { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string Specialty { get; set; } = string.Empty;

        public int Experience { get; set; }

        public string? ProfileImage { get; set; } = string.Empty;
    }

    
}
