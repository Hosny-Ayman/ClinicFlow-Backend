using AutoMapper;
using ClinicFlow.Application.Features.Authentication.DTOs.Requests;
using ClinicFlow.Application.Features.Authentication.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.Authentication
{
    public class AuthenticationProfile:Profile
    {

        public AuthenticationProfile()
        {
            CreateMap<User, LoginDtoRequest>().ReverseMap();

            CreateMap<LoginDtoResponse,User>().ReverseMap()
                .ForMember(d => d.Roles,
                opt => opt.MapFrom(s => s.UserRoles.Select(r => Enum.Parse<RoleEnum>(r.Role.Name))));

        }

    }
}
