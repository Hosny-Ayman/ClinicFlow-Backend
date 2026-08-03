using AutoMapper;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Application.Features.Doctors.DTOs.Responses;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.Doctors
{
    public class DoctorProfile:Profile
    {

        public DoctorProfile()
        {
            CreateMap<CreateAndEditDoctorDtoRequest, Doctor>().ReverseMap().ForMember(dest=>dest.ProfileImage,opt=>opt.Ignore());

            CreateMap<GetDoctorInforamtionDtoResponse, Doctor>().ReverseMap();

            CreateMap<UpdateDoctorInforamtionDtoRequest, Doctor>().ReverseMap();
        }

    }
}
