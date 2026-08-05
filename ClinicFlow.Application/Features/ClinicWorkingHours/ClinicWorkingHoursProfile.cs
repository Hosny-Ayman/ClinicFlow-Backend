using AutoMapper;
using ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Requests;
using ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Responses;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.ClinicWorkingHours
{
    public class ClinicWorkingHoursProfile:Profile
    {

        public ClinicWorkingHoursProfile()
        {

            CreateMap<CreateClinicWorkingHourDtoRequest, ClinicWorkingHour>().ReverseMap();
            CreateMap<GetAllWorkingHoursAndDaysDtoResponse, ClinicWorkingHour>().ReverseMap();
            CreateMap<UpdateClinicWorkingHoursAndDaysDtoRequest, ClinicWorkingHour>().ReverseMap();
        }
    }
}
