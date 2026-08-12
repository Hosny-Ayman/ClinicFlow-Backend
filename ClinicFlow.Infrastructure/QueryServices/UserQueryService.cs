using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Features.Authentication.DTOs.Responses;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Application.Features.Users.DTOs.Responses;
using ClinicFlow.Domain.Enums;
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

        public async Task<PagedResponse<GetAllReceptionistsDtoRequest>> GetAllReceptionistsformationsAsync(ReceptionistsSearchDtoRequest request, int clinicId)
        {
            var query = _appDbContext.Users.Include(x => x.Person).Where(x => x.ClinicId == clinicId && x.UserRoles.Any(x=>x.RoleId ==(int) RoleEnum.Receptionist)).AsNoTracking().AsQueryable();



            if (!string.IsNullOrWhiteSpace(request.FullNameSearch))
            {
                var search = request.FullNameSearch.Trim();
                query = query.Where(x => x.Person.FirstName.Contains(search) || x.Person.LastName.Contains(search) ||
                (x.Person.FirstName + " " + x.Person.LastName).Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.EmailSearch))
            {
                query = query.Where(x => x.Person.Email != null && x.Person.Email.Contains(request.EmailSearch));
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumberSearch))
            {
                query = query.Where(x => x.Person.PhoneNumber != null && x.Person.PhoneNumber.Contains(request.PhoneNumberSearch));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.IsActive == request.Status);
            }




            var totalrecords = await query.CountAsync();

            if (!string.IsNullOrEmpty(request.SortField))
            {
                switch (request.SortField.ToLower())
                {
                    case "fullname":
                        query = request.SortOrder == -1 ?
                            query.OrderByDescending(x => (x.Person.FirstName + " " + x.Person.LastName)) :
                            query.OrderBy(x => (x.Person.FirstName + " " + x.Person.LastName));
                        break;


                    case "email":
                        query = request.SortOrder == -1 ?
                            query.OrderByDescending(x => x.Person.Email) :
                            query.OrderBy(x => x.Person.Email);
                        break;

                    case "phonenumber":
                        query = request.SortOrder == -1 ?
                            query.OrderByDescending(x => x.Person.PhoneNumber) :
                            query.OrderBy(x => x.Person.PhoneNumber);
                        break;

                    case "status":
                        query = request.SortOrder == -1 ?
                            query.OrderByDescending(x => x.IsActive) :
                            query.OrderBy(x => x.IsActive);
                        break;
                }
            }

            var data = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).Select(x => new GetAllReceptionistsDtoRequest
            {

                Id = x.Id,
                FullName = $"{x.Person.FirstName} {x.Person.LastName}",
                Email = x.Person.Email ?? "",
                PhoneNumber = x.Person.PhoneNumber ?? "",
                Status = x.IsActive ? "Active" : "Inactive",

            }
            ).ToListAsync();

           

            return new PagedResponse<GetAllReceptionistsDtoRequest>(data, totalrecords, request.PageNumber, request.PageSize);
        }

        public async Task<CurrentUserDto?> GetUserProfilByEmaileAsync(string Email)
        {
            var user = await _appDbContext.Users
                .Include(x => x.Person)
                .Where(x => x.Person.Email == Email)
                .Select(x => new
                {
                    x.Id,
                    FullName = x.Person.FirstName + " " + x.Person.LastName,
                    Email = x.Person.Email,
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
                Email = user.Email ?? "",
                ClinicId = user.ClinicId,

                Roles = user.Roles,

                Permissions = user.Permissions
                    .Aggregate(0L, (current, permission) => current | permission)
            };
        }
    }
}
