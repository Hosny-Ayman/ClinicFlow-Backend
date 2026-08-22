using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Specifications;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Application.Features.Doctors.DTOs.Responses;
using ClinicFlow.Application.Features.Doctors.DTOs.Specifications;
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

        public async Task<PagedResponse<GetAllDoctorsInformationsDtoResponse>> GetAllDoctorsInformationsAsync(DoctorSearchDtoRequest request, int clinicId)
        {
            var query = _appDbContext.Doctors.AsNoTracking().AsQueryable();

            var spec = new DoctorsWithSearchSpecification(request, clinicId);

            query = SpecificationEvaluator.GetQuery(query, spec);

            var totalrecords = await query.CountAsync();

            var data = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new GetAllDoctorsInformationsDtoResponse
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
                })
                .ToListAsync();

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