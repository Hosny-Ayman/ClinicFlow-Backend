using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Common.Security
{
    public class CheckService: ICheckService
    {

        private readonly ICurrentUserService _currentUser;

        public CheckService(ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
        }

        public bool EnsureCanManageUser(int targetUserId)
        {
            if (_currentUser.Roles.Contains(nameof(RoleEnum.ClinicOwner)))
                return true;

            if (_currentUser.Roles.Contains(nameof(RoleEnum.SuperAdmin)))
                return true;

            if (_currentUser.UserId == targetUserId)
                return true;

            return false;
        }
    }
}
