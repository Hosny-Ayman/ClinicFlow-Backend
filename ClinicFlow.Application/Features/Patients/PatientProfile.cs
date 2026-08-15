using AutoMapper;
using ClinicFlow.Application.Features.Patients.DTOs.Requests;
using ClinicFlow.Application.Features.Patients.DTOs.Responses;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.Patients
{
    public class PatientProfile : Profile
    {

        public PatientProfile()
        {
            CreateMap<CreatePatientDtoRequest, Patient>().ReverseMap();

            CreateMap<UpdatePatientDtoRequest, Patient>().ReverseMap();

            CreateMap<Patient, GetPatientDtoResponse>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Person.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Person.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Person.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Person.PhoneNumber));
        }

    }
}
