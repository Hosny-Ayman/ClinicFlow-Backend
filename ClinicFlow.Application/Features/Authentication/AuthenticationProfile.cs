using AutoMapper;
using ClinicFlow.Application.Features.Authentication.DTOs.Requests;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.Authentication
{
    public class AuthenticationProfile:Profile
    {

        public AuthenticationProfile()
        {
            CreateMap<User,LoginDtoRequest>().ReverseMap();
        }

    }
}
