namespace ClinicFlow.Application.Common.Security
{
    public interface IAuthorizationService
    {

        bool EnsureCanManageUser(int targetUserId);

    }
}
