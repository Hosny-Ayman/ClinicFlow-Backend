using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.Doctors.DTOs.Responses
{
    public sealed record GetDoctorInforamtionDtoResponse
    {

        public int Id { get; set; }
        public string SpecialtieName { get; set; } = null!;
        public decimal ConsultationFee { get; set; }
        public string? Bio { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public int ExperienceYears { get; set; }
        public string? ProfileImageUrl { get; set; } = null!;
    }
}
