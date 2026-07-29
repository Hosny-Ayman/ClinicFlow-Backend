using AutoMapper;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.Doctors
{
    public class DoctorProfile:Profile
    {

        public DoctorProfile()
        {
            CreateMap<CreateDoctorDtoRequest, Doctor>().ReverseMap().ForMember(dest=>dest.ProfileImage,opt=>opt.Ignore());
        }

    }
}
