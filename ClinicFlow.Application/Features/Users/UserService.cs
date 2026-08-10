using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Common.Security;
using ClinicFlow.Application.Features.Authentication.DTOs.Responses;
using ClinicFlow.Application.Features.Doctors.DTOs.Responses;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Application.Features.Users.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.InterFaces;

namespace ClinicFlow.Application.Features.Users
{
    public class UserService
    {

        private readonly IUserQueryService _userQueryService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly ICheckService _authorizationService;

        public UserService(IUserQueryService userQueryService, ICurrentUserService currentUserService,
          IUserRepository userRepository, IMapper mapper, IUserRoleRepository userRoleRepository,
          IUnitOfWork unitOfWork, ICheckService authorizationService)
        {
            _userQueryService = userQueryService;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _mapper = mapper;
            _userRoleRepository = userRoleRepository;
            _UnitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }

        public async Task<OperationResult<CurrentUserDto>> GetCurrentUserAsync()
        {
            if (!_currentUserService.IsAuthenticated)
            {
                return OperationResult<CurrentUserDto>.Unauthorized();
            }

            var CurrentUser = await _userQueryService.GetUserProfilByEmaileAsync(_currentUserService.Email!);

            if (CurrentUser == null)
            {
                return OperationResult<CurrentUserDto>.NotFound();
            }

            return OperationResult<CurrentUserDto>.Success(CurrentUser!);


        }

        public async Task<OperationResult<int>> CreateReceptionistAsync(CreateAndEditUserDtoRequest userDto)
        {      

            var user = await AddUserInsideProjectOnlyAsync(userDto);

            if(user == null)
            {
                return OperationResult<int>.Conflict(GeneralErrors.Conflict("The User Is Already Exists"));
            }

            await _userRoleRepository.AssignRoleAsync(user, RoleEnum.Receptionist);
           
            await _UnitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(user.Id);
           
        }

        public async Task<User?> AddUserInsideProjectOnlyAsync(CreateAndEditUserDtoRequest userDto)
        {

            if(await _userRepository.IsEmailExitsAsync(userDto.Email) || await _userRepository.IsPhoneExitsAsync(userDto.PhoneNumber))
            {
                return null;
            }

            var user = _mapper.Map<User>(userDto);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            if (_currentUserService.ClinicId is int clinicId)
            {
                user.ClinicId = clinicId;
            }

           

            await _userRepository.AddAsync(user);

            return user;

        }

        public async Task<OperationResult<GetUserInformationDtoResponse>> GetUserInformationByIdAsync(int userId)
        {

            if (!_authorizationService.EnsureCanManageUser(userId))
            {
                return OperationResult<GetUserInformationDtoResponse>.Forbidden();
            }

            var user = await _userRepository.GetUserByIdAsync(userId, _currentUserService.ClinicId!.Value);

            if(user == null)
            {
                return OperationResult<GetUserInformationDtoResponse>.NotFound();
            }

            var userDto = _mapper.Map<GetUserInformationDtoResponse>(user);

            return OperationResult<GetUserInformationDtoResponse>.Success(userDto);
        }

        public async Task<OperationResult<bool>> UpdateUserAsync(UpdateUserInformationDtoRequest userDto)
        {

            if (!_authorizationService.EnsureCanManageUser(userDto.Id))
            {
                return OperationResult<bool>.Forbidden();
            }

            var user = await _userRepository.GetUserByIdAsync(userDto.Id, _currentUserService.ClinicId!.Value, true);

            if(user == null)
            {
                return OperationResult<bool>.NotFound(GeneralErrors.NotFound("Update Failed User Not Found"));
            }

            UpdateUserInsideProjectOnlyAsync(user, userDto);

            await _UnitOfWork.SaveChangesAsync();

            return OperationResult<bool>.Success(true);

        }

        public void UpdateUserInsideProjectOnlyAsync(User user, UpdateUserInformationDtoRequest dto)
        {
            var oldPassword = user.PasswordHash;

            _mapper.Map(dto, user);

            user.PasswordHash = string.IsNullOrWhiteSpace(dto.Password)? oldPassword: BCrypt.Net.BCrypt.HashPassword(dto.Password);

          
        }
    }
}
