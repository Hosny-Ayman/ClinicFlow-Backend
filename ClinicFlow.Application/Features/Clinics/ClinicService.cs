using AutoMapper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Clinics.DTOs.Requests;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.InterFaces;

namespace ClinicFlow.Application.Features.Clinics
{
    public class ClinicService
    {

        private readonly IClinicRepository _clinicRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IOwnershipService _ownershipService;
        private readonly IUnitOfWork _unitOfWork;


        public ClinicService(IClinicRepository clinicRepository,IUserRepository userRepository,IMapper mapper, IOwnershipService ownershipService
            , IUnitOfWork unitOfWork)
        {
            _clinicRepository = clinicRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _ownershipService = ownershipService;
            _unitOfWork = unitOfWork;
        }

      

        public async Task<OperationResult<int>> CreateClinicAsync(CreateClinicDtoRequest clinicDto, CreateUserDtoRequest userDto)
        {

            var clinic = _mapper.Map<Clinic>(clinicDto);

            var user = _mapper.Map<User>(userDto);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            user.UserRoles.Add(new UserRole
            {
                RoleId = (int)RoleEnum.ClinicOwner,
              
            });

            user.Clinic = clinic;

            await _clinicRepository.AddAsync(clinic);

            await _userRepository.AddAsync(user);

            await _unitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(clinic.Id);

        }


    }
}
