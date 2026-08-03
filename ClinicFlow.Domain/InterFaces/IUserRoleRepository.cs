using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Domain.InterFaces
{
    public interface IUserRoleRepository
    {

        Task AssignRoleAsync(int userId, RoleEnum role);

        Task RemoveRoleAsync(int userId, RoleEnum role);

        Task<bool> HasRoleAsync(int userId, RoleEnum role);

      

    }
}
