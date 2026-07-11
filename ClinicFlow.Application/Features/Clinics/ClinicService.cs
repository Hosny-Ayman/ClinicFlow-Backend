using AutoMapper;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Clinics.DTOs.Requests;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.Clinics
{
    public class ClinicService
    {

        private readonly IClinicRepository _clinicRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ClinicService(IClinicRepository clinicRepository,IUserRepository userRepository,IMapper mapper)
        {
            _clinicRepository = clinicRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

      

        public async Task<OperationResult<int>> CreateClinicAsync(CreateClinicDtoRequest clinicDto, CreateUserDtoRequest userDto)
        {

            var clinic = _mapper.Map<Clinic>(clinicDto);
            var user = _mapper.Map<User>(userDto);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
           


            var clinicId = await _clinicRepository.AddAsync(clinic, user);

            return OperationResult<int>.Success(clinicId);

        }


    }
}
