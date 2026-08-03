using AutoMapper;
using ClinicFlow.Application.Features.Specialties.DTOs.Requests;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.Specialties
{
    public class SpecialityProfile:Profile
    {

        public SpecialityProfile() 
        {

            CreateMap<GetAllSpecialityDtoRequest, Specialty>().ReverseMap();


        }

    }
}
