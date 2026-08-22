using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Specifications;
using ClinicFlow.Application.Features.DoctorVacations.DTOs.Requests;
using ClinicFlow.Application.Features.DoctorVacations.DTOs.Responses;
using ClinicFlow.Application.Features.DoctorVacations.DTOs.Specifications;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.QueryServices
{
    public class DoctorVacationQueryService : IDoctorVacationQueryService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileStorageService _fileStorageService;

        public DoctorVacationQueryService(AppDbContext appDbContext, IFileStorageService fileStorageService)
        {
            _appDbContext = appDbContext;
            _fileStorageService = fileStorageService;
        }

        public async Task<PagedResponse<GetAllDoctorVacationInformationDtoResponse>> GetAllDoctorVacationInformationAsync(DoctorVacationSearchDtoRequest request, int clinicId)
        {
            var query = _appDbContext.DoctorVacations.AsNoTracking().AsQueryable();

            var spec = new DoctorVacationWithSearchSpecification(request, clinicId);
            query = SpecificationEvaluator.GetQuery(query, spec);

            var totalrecords = await query.CountAsync();

            var data = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(v => new GetAllDoctorVacationInformationDtoResponse
                {
                    Id = v.Id,
                    UserId = v.Doctor.UserId,
                    FullName = $"{v.Doctor.User.Person.FirstName} {v.Doctor.User.Person.LastName}",
                    Email = v.Doctor.User.Person.Email ?? "",
                    PhoneNumber = v.Doctor.User.Person.PhoneNumber ?? "",
                    Gender = v.Doctor.Gender.ToString(),
                    Specialty = v.Doctor.Specialty.Name,
                    Experience = v.Doctor.ExperienceYears,
                    ProfileImage = v.Doctor.ProfileImageUrl,
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    Reason = v.Reason,
                    Status = v.Status.ToString()
                })
                .ToListAsync();

            foreach (var item in data)
            {
                if (!string.IsNullOrEmpty(item.ProfileImage))
                {
                    item.ProfileImage = _fileStorageService.GetFileUrl(item.ProfileImage);
                }
            }

            return new PagedResponse<GetAllDoctorVacationInformationDtoResponse>(data, totalrecords, request.PageNumber, request.PageSize);
        }

        public async Task<GetDoctorVacationDashboardInformationDtoResponse> GetDoctorVacationDashboardInformationAsync(int clinicId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var result = await _appDbContext.DoctorVacations
                .Where(v => v.Doctor.ClinicId == clinicId)
                .GroupBy(x => 1)
                .Select(d => new GetDoctorVacationDashboardInformationDtoResponse
                {
                    TotalLeavesCount = d.Count(),
                    UpcomingLeavesCount = d.Count(s => s.StartDate > today),
                    ActiveLeavesCount = d.Count(s => s.StartDate <= today && s.EndDate >= today),
                    CompletedLeavesCount = d.Count(s => s.EndDate < today)
                })
                .FirstOrDefaultAsync();

            return result ?? new GetDoctorVacationDashboardInformationDtoResponse();
        }
    }
}