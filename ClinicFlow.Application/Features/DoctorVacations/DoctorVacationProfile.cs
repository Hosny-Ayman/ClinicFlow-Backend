using AutoMapper;
using ClinicFlow.Application.Features.DoctorVacations.DTOs;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.DoctorVacations
{
    public class DoctorVacationProfile:Profile
    {

        public DoctorVacationProfile()
        {
            CreateMap<DoctorVacation, Get_Create_Update_DoctorVacationDto>().ReverseMap();
        }


    }
}
