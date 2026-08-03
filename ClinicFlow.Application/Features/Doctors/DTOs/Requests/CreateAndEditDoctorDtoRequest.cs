using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace ClinicFlow.Application.Features.Doctors.DTOs.Requests
{
    public sealed record CreateAndEditDoctorDtoRequest
    {

       
        public int SpecialtyId { get; init; }
        public decimal ConsultationFee { get; init; }
        public string? Bio { get; init; }
        public GenderEnum Gender { get; init; }
        public int ExperienceYears { get; init; }
        public IFormFile? ProfileImage { get; init; }



            
    }
}
