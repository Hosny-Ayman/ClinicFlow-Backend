using AutoMapper;
using ClinicFlow.Application.Features.Clinics.DTOs.Requests;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.Clinics
{
    public class ClinicProfile:Profile
    {
         public ClinicProfile()
        {
            CreateMap<CreateClinicDtoRequest,Clinic>().ReverseMap();
        }


    }
}
