using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Features.Authentication.DTOs.Responses;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace ClinicFlow.Infrastructure.QueryServices
{
    public class UserQueryService : IUserQueryService
    {
        private readonly AppDbContext _appDbContext;

        public UserQueryService(AppDbContext appDbContext) 
        {
            _appDbContext = appDbContext;

        }

        public async Task<CurrentUserDto?> GetUserProfilByEmaileAsync(string Email)
        {
            var user = await _appDbContext.Users
                .Where(x => x.Email == Email)
                .Select(x => new
                {
                    x.Id,
                    FullName = x.FirstName + " " + x.LastName,
                    x.Email,
                    x.ClinicId,

                    Roles = x.UserRoles
                        .Select(ur => ur.Role.Name)
                        .ToList(),

                    Permissions = x.UserRoles
                        .Select(ur => ur.Role.Permissions)
                        .ToList()
                })
                .SingleOrDefaultAsync();


            if (user == null)
                return null;


            return new CurrentUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                ClinicId = user.ClinicId,

                Roles = user.Roles,

                Permissions = user.Permissions
                    .Aggregate(0L, (current, permission) => current | permission)
            };
        }
    }
}
