using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Features.Authentication.DTOs.Responses;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
           return await _appDbContext.Users.Where(x=>x.Email == Email).Select(x=> new CurrentUserDto
           {
              Id = x.Id,
              FullName = x.FirstName +" "+x.LastName,
              Email = x.Email,
              ClinicId = x.ClinicId,
              Roles = x.UserRoles.Select(x=> x.Role.Name).ToList()
           }).SingleOrDefaultAsync();
         
        }
    }
}
