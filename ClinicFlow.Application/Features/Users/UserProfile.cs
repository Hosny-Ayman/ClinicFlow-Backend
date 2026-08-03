using AutoMapper;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Application.Features.Users.DTOs.Responses;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateAndEditUserDtoRequest, User>().ReverseMap();

            CreateMap<GetUserInformationDtoResponse, User>().ReverseMap();

            CreateMap<UpdateUserInformationDtoRequest, User>().ReverseMap();
        }


    }
}
