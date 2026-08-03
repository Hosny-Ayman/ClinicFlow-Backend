using AutoMapper;
using ClinicFlow.Application.Features.ClinicSetups.DTOs.Requests;
using ClinicFlow.Application.Features.ClinicSetups.DTOs.Responses;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.ClinicSetups
{
    public class ClinicSetupProfile:Profile
    {


        public ClinicSetupProfile() 
        {

            CreateMap<CreateClinicSetupDtoRequest, ClinicSetup>().ReverseMap();
            CreateMap<GetClinicSetupStatusDtoResponse, ClinicSetup>().ReverseMap();

        }

    }
}
