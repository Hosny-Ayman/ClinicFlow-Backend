namespace ClinicFlow.Application.Common.Security
{
    public interface ICheckService
    {

        bool EnsureCanManageUser(int targetUserId);

    }
}
