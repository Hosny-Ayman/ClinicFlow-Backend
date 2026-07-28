using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Clinics.DTOs.Requests;
using ClinicFlow.Application.Features.Clinics.DTOs.Responses;
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
        private readonly IClinicQueryService _queryService;


        public ClinicService(IClinicRepository clinicRepository,IUserRepository userRepository,IMapper mapper, IOwnershipService ownershipService
            , IUnitOfWork unitOfWork, IClinicQueryService QueryService)
        {
            _clinicRepository = clinicRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _ownershipService = ownershipService;
            _unitOfWork = unitOfWork;
            _queryService = QueryService;
        }

      

        public async Task<OperationResult<CreateClinicResponse>> CreateClinicAsync(CreateClinicDtoRequest clinicDto, CreateUserDtoRequest userDto)
        {
            if(await _userRepository.IsEmailExitsAsync(userDto.Email))
            {
                return OperationResult<CreateClinicResponse>.Conflict(GeneralErrors.Conflict("Email Is Already Exits"));
            }

            if (await _userRepository.IsPhoneExitsAsync(userDto.PhoneNumber))
            {
                return OperationResult<CreateClinicResponse>.Conflict(GeneralErrors.Conflict("Phone Number Is Already Exits"));
            }

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

            var requst = await _queryService.GetClinicInfoWithOwnerFullnameAsync(clinic.Id);

            if (requst == null)
            {
                return OperationResult<CreateClinicResponse>.Failure(GeneralErrors.Failure("Create Clinic Failed Try Later"));
            }

            return OperationResult<CreateClinicResponse>.Success(requst);

        }


    }
}
