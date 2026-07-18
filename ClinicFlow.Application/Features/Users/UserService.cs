using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Authentication.DTOs.Responses;

namespace ClinicFlow.Application.Features.Users
{
    public class UserService
    {

        private readonly IUserQueryService _userQueryService;
        private readonly ICurrentUserService _currentUserService;

        public UserService(IUserQueryService userQueryService, ICurrentUserService currentUserService)
        {
            _userQueryService = userQueryService;
            _currentUserService = currentUserService;
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
    }
}
