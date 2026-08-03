using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Authentication.DTOs.Responses;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.InterFaces;
using System.Threading.Tasks;

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

      public UserService(IUserQueryService userQueryService, ICurrentUserService currentUserService,
          IUserRepository userRepository, IMapper mapper, IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork)
        {
            _userQueryService = userQueryService;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _mapper = mapper;
            _userRoleRepository = userRoleRepository;
            _UnitOfWork = unitOfWork;
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

            var user = await AddUserAsync(userDto);

            if(user == null)
            {
                return OperationResult<int>.Conflict(GeneralErrors.Conflict("The User Is Already Exists"));
            }

            await _userRoleRepository.AssignRoleAsync(user.Id, RoleEnum.Receptionist);
           
            await _UnitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(user.Id);
           
        }

        public async Task<User?> AddUserAsync(CreateAndEditUserDtoRequest userDto)
        {

            if(await _userRepository.IsEmailExitsAsync(userDto.Email) || await _userRepository.IsPhoneExitsAsync(userDto.PhoneNumber))
            {
                return null;
            }

            var user = _mapper.Map<User>(userDto);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            user.ClinicId = _currentUserService.ClinicId!.Value;

            await _userRepository.AddAsync(user);

            return user;

        }

        public async Task UpdateUserAsync(User user, UpdateUserInformationDtoRequest dto)
        {
            var oldPassword = user.PasswordHash;

            _mapper.Map(dto, user);

            user.PasswordHash = string.IsNullOrWhiteSpace(dto.Password)? oldPassword: BCrypt.Net.BCrypt.HashPassword(dto.Password);

          
        }
    }
}
