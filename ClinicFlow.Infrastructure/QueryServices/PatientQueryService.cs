using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Features.Patients.DTOs.Requests;
using ClinicFlow.Application.Features.Patients.DTOs.Responses;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.QueryServices
{
    public class PatientQueryService : IPatientQueryService
    {
        private readonly AppDbContext _appDbContext;

        public PatientQueryService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public async Task<PagedResponse<GetAllPatientsDtoResponse>> GetAllPatientsAsync(PatientSearchDtoRequest request, int clinicId)
        {
            var query = _appDbContext.Patients
                .Where(p => p.ClinicPatients.Any(cp => cp.ClinicId == clinicId))
                .Include(p => p.Person)
                .Include(p => p.ClinicPatients)
                .AsNoTracking()
                .AsQueryable();


            if (!string.IsNullOrWhiteSpace(request.FullNameSearch))
            {
                var search = request.FullNameSearch.Trim();
                query = query.Where(p =>
                    p.Person.FirstName.Contains(search) ||
                    p.Person.LastName.Contains(search) ||
                    (p.Person.FirstName + " " + p.Person.LastName).Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumberSearch))
            {
                query = query.Where(p => p.Person.PhoneNumber != null && p.Person.PhoneNumber.Contains(request.PhoneNumberSearch));
            }

            if (!string.IsNullOrWhiteSpace(request.NationalIdSearch))
            {
                query = query.Where(p => p.NationalId != null && p.NationalId.Contains(request.NationalIdSearch));
            }

            if (request.Gender.HasValue)
            {
                query = query.Where(p => p.Gender == request.Gender);
            }

            if (request.BloodType.HasValue)
            {
                query = query.Where(p => p.BloodType == request.BloodType);
            }


            var totalRecords = await query.CountAsync();

            if (!string.IsNullOrEmpty(request.SortField))
            {
                switch (request.SortField.ToLower())
                {
                    case "fullname":
                        query = request.SortOrder == -1
                            ? query.OrderByDescending(p => p.Person.FirstName + " " + p.Person.LastName)
                            : query.OrderBy(p => p.Person.FirstName + " " + p.Person.LastName);
                        break;

                    case "dateofbirth":
                        query = request.SortOrder == -1
                            ? query.OrderByDescending(p => p.DateOfBirth)
                            : query.OrderBy(p => p.DateOfBirth);
                        break;

                    case "gender":
                        query = request.SortOrder == -1
                            ? query.OrderByDescending(p => p.Gender)
                            : query.OrderBy(p => p.Gender);
                        break;

                    case "createdat":
                        query = request.SortOrder == -1
                            ? query.OrderByDescending(p => p.CreatedAt)
                            : query.OrderBy(p => p.CreatedAt);
                        break;

                    case "status":
                        query = request.SortOrder == -1
                            ? query.OrderByDescending(p => p.ClinicPatients.Any(cp => cp.ClinicId == clinicId && cp.IsActive))
                            : query.OrderBy(p => p.ClinicPatients.Any(cp => cp.ClinicId == clinicId && cp.IsActive));
                        break;
                }
            }

            var data = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new GetAllPatientsDtoResponse
                {
                    Id = p.Id,
                    FullName = $"{p.Person.FirstName} {p.Person.LastName}",
                    PhoneNumber = p.Person.PhoneNumber,
                    NationalId = p.NationalId,
                    Gender = p.Gender.ToString(),
                    DateOfBirth = p.DateOfBirth,
                    BloodType = p.BloodType != null ? p.BloodType.ToString() : null,
                    Status = p.ClinicPatients.Any(cp => cp.ClinicId == clinicId && cp.IsActive) ? "Active" : "Inactive"
                })
                .ToListAsync();

            return new PagedResponse<GetAllPatientsDtoResponse>(data, totalRecords, request.PageNumber, request.PageSize);
        }
    }
}
