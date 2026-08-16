using AutoMapper;
using ClinicFlow.Application.Features.DoctorSchedules.DTOs.Requests;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.DoctorSchedules
{
    public class DoctorScheduleProfile:Profile
    {
        public DoctorScheduleProfile()
        {
            CreateMap<UpdateAndGetDoctorScheduleDtoRequest, DoctorSchedule>().ReverseMap();
        }

    }
}
