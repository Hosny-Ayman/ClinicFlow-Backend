using AutoMapper;
using ClinicFlow.Application.Features.Clinics.DTOs.Requests;
using ClinicFlow.Application.Features.Clinics.DTOs.Responses;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.Clinics
{
    public class ClinicProfile:Profile
    {
        public ClinicProfile()
        {
            CreateMap<CreateAndEditClinicDtoRequest,Clinic>().ReverseMap();
            CreateMap<GetClinicDtoResponse, Clinic>().ReverseMap();
        }


    }
}
