using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Infrastructure.Data;

namespace ClinicFlow.Infrastructure.Repositories
{
    internal class UserRoleRepository : IUserRoleRepository
    {
        private readonly IUserRepository _userRepository;

        public UserRoleRepository(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AssignRoleAsync(int userId, RoleEnum role)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, true);

            if (user == null)
                throw new Exception("User not found");


            var roleId = (int)role;


            if (user.UserRoles.Any(x => x.RoleId == roleId))
                return;


            user.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = roleId
            });
        }

        public async Task<bool> HasRoleAsync(int userId, RoleEnum role)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, true);

            if (user == null)
                return false;


            return user.UserRoles.Any(x => x.RoleId == (int)role);
        }

        public async Task RemoveRoleAsync(int userId, RoleEnum role)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, true);

            if (user == null)
                throw new Exception("User not found");


            var userRole = user.UserRoles.FirstOrDefault(x => x.RoleId == (int)role);


            if (userRole != null)
            {
                user.UserRoles.Remove(userRole);
            }
        }
    }
}
