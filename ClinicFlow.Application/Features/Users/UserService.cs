using AutoMapper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Authentication.DTOs.Responses;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
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

        public async Task<OperationResult<int>> CreateReceptionistAsync(CreateUserDtoRequest userDto)
        {
            
            var user = _mapper.Map<User>(userDto);
           
            await _userRepository.AddAsync(user);
           
            await _userRoleRepository.AssignRoleAsync(user.Id, RoleEnum.Receptionist);
           
            await _UnitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(user.Id);
           
        }
    }
}
