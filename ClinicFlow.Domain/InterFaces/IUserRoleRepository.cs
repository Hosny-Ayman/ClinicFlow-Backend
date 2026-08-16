using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Domain.Interfaces
{
    public interface IUserRoleRepository
    {

        Task AssignRoleAsync(User user, RoleEnum role);

        Task RemoveRoleAsync(int userId, RoleEnum role);

        Task<bool> HasRoleAsync(int userId, RoleEnum role);

      

    }
}
