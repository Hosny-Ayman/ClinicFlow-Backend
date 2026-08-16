using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    internal class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRoleRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AssignRoleAsync(User user, RoleEnum role)
        {

            var roleId = (int)role;

            var userRole = new UserRole
            {
                User = user,
                RoleId = (int)role
            };

            await _appDbContext.UserRoles.AddAsync(userRole);
        }

        public async Task<bool> HasRoleAsync(int userId, RoleEnum role)
        {
            var userRoles = await _appDbContext.UserRoles.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();

            if (userRoles == null || userRoles.Count == 0)
                return false;


            return  userRoles.Any(x => x.RoleId == (int)role);
        }

        public async Task RemoveRoleAsync(int userId, RoleEnum role)
        {
            var userRole = await _appDbContext.UserRoles.SingleOrDefaultAsync(x => x.UserId == userId && x.RoleId == (int)role);

            if(userRole!=null)
            _appDbContext.UserRoles.Remove(userRole);

           
        }

        
    }
}
