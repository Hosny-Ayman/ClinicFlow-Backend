using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Features.Clinics.DTOs.Responses;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.QueryServices
{
    public class ClinicQueryService : IClinicQueryService
    {
        private readonly AppDbContext _appDbContext;

        public ClinicQueryService(AppDbContext AppDbContext)
        {
            _appDbContext = AppDbContext;
        }



        public async Task<CreateClinicResponse?> GetClinicInfoWithOwnerFullnameAsync(int ClinicId)
        {
            return await _appDbContext.Clinics.AsNoTracking().Where(x => x.Id == ClinicId).Select(x => new CreateClinicResponse {
                ClinicId = x.Id,
                ClinicName = x.Name,
                ClinicEmail = x.Email,
                ClinicPhone = x.Phone,
                ClinicAddress = x.Address,
                CreatedAt = x.CreatedAt,
                OwnerFullName = x.Users.Where(x => x.UserRoles.Any(y => y.Role.Name == RoleEnum.ClinicOwner.ToString())).Select(x=>$"{x.Person.FirstName} {x.Person.LastName}" ).First()

           }).FirstOrDefaultAsync();
        }
    }
}
