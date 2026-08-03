using ClinicFlow.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace ClinicFlow.Application.Features.Doctors.DTOs.Requests
{
    public sealed record UpdateDoctorInforamtionDtoRequest
    {
        public int Id { get; init; }
        public int SpecialtyId { get; set; } 
        public decimal ConsultationFee { get; init; }
        public string? Bio { get; init; } = null!;
        public GenderEnum Gender { get; set; } 
        public int ExperienceYears { get; init; }
        public IFormFile? ProfileImageUrl { get; init; } = null!;
        public bool IsImageDeleted { get; set; } = false;

    }
}
