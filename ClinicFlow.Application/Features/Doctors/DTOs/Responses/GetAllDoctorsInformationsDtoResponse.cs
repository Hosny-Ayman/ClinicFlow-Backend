using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.Doctors.DTOs.Responses
{
    public sealed record GetAllDoctorsInformationsDtoResponse
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Gender { get; set; }  = string.Empty;

        public string Specialty { get; set; } = string.Empty;

        public int Experience { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Image { get; set; } = string.Empty;


    }
}
