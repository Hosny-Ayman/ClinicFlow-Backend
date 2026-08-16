using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Application.Features.Doctors.DTOs.Responses;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.QueryServices
{
    public class DoctorQueryService : IDoctorQueryService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileStorageService _fileStorageService;

        public DoctorQueryService(AppDbContext appDbContext, IFileStorageService fileStorageService)
        {
            _appDbContext = appDbContext;
            _fileStorageService = fileStorageService;
        }


        public async Task<PagedResponse<GetAllDoctorsInformationsDtoResponse>> GetAllDoctorsInformationsAsync(DoctorSearchDtoRequest request,int clinicId)
        {
            var query = _appDbContext.Doctors.Include(x => x.User).ThenInclude(x => x.Person).Where(x=>x.ClinicId == clinicId).AsNoTracking().AsQueryable();

            

            if (!string.IsNullOrWhiteSpace(request.FullNameSearch))
            {
                var search = request.FullNameSearch.Trim();
                query = query.Where(x =>x.User.Person.FirstName.Contains(search) ||x.User.Person.LastName.Contains(search) ||
                (x.User.Person.FirstName + " " + x.User.Person.LastName).Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.EmailSearch))
            {
                query = query.Where(x => x.User.Person.Email != null && x.User.Person.Email.Contains(request.EmailSearch));
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumberSearch))
            {
                query = query.Where(x => x.User.Person.PhoneNumber != null && x.User.Person.PhoneNumber.Contains(request.PhoneNumberSearch));
            }

            if (request.Gender.HasValue)
            {
                query = query.Where(x => x.Gender == request.Gender);
            }

            if (request.SpecialtyId.HasValue)
            {
                query = query.Where(x => x.SpecialtyId == request.SpecialtyId);
            }


            var totalrecords = await query.CountAsync();

            if (!string.IsNullOrEmpty(request.SortField))
            {
                switch(request.SortField.ToLower())
                {
                    case "fullname":
                        query = request.SortOrder == -1 ?
                            query.OrderByDescending(x => (x.User.Person.FirstName + " " + x.User.Person.LastName)):
                            query.OrderBy(x => (x.User.Person.FirstName + " " + x.User.Person.LastName));
                        break;


                    case "email":
                        query = request.SortOrder == -1 ?
                            query.OrderByDescending(x => x.User.Person.Email):
                            query.OrderBy(x => x.User.Person.Email);
                        break;

                    case "phonenumber":
                        query = request.SortOrder == -1 ?
                            query.OrderByDescending(x => x.User.Person.PhoneNumber):
                            query.OrderBy(x => x.User.Person.PhoneNumber);
                        break;

                    case "specialty":
                        query = request.SortOrder == -1 ?
                             query.OrderByDescending(x => x.Specialty.Name):
                            query.OrderBy(x => x.Specialty.Name);
                        break;

                    case "experience":
                        query = request.SortOrder == -1 ?
                            query.OrderByDescending(x => x.ExperienceYears):
                            query.OrderBy(x => x.ExperienceYears);
                        break;

                    case "status":
                        query = request.SortOrder == -1 ?
                            query.OrderByDescending(x => x.User.IsActive):
                            query.OrderBy(x => x.User.IsActive);
                        break;
                }
            }

            var data = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).Select(x => new GetAllDoctorsInformationsDtoResponse
            {

                Id = x.Id,
                UserId = x.UserId,
                FullName = $"{x.User.Person.FirstName} {x.User.Person.LastName}",
                Email = x.User.Person.Email ?? "",
                PhoneNumber = x.User.Person.PhoneNumber ?? "",
                Gender = x.Gender.ToString(),
                Specialty = x.Specialty.Name,
                Experience = x.ExperienceYears,
                Status = x.User.IsActive ? "Active" : "Inactive",
                Image = x.ProfileImageUrl

            }
            ).ToListAsync();

            foreach (var doctor in data)
            {
                if (!string.IsNullOrEmpty(doctor.Image))
                {
                    doctor.Image = _fileStorageService.GetFileUrl(doctor.Image);
                }
            }


            return new PagedResponse<GetAllDoctorsInformationsDtoResponse>(data, totalrecords, request.PageNumber, request.PageSize);


        }

     
    }
}
