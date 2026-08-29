using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Specifications; 
using ClinicFlow.Application.Features.Patients.DTOs.Requests;
using ClinicFlow.Application.Features.Patients.DTOs.Responses;
using ClinicFlow.Application.Features.Patients.DTOs.Specifications; 
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
            var query = _appDbContext.Patients.AsNoTracking().AsQueryable();

            var spec = new PatientWithSearchSpecification(request, clinicId);
            query = SpecificationEvaluator.GetQuery(query, spec);

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new GetAllPatientsDtoResponse
                {
                    Id = p.Id,
                    FullName = $"{p.Person.FirstName} {p.Person.LastName}",
                    PhoneNumber = p.Person.PhoneNumber ?? "",
                    NationalId = p.NationalId ?? "",
                    Gender = p.Gender.ToString(),
                    DateOfBirth = p.DateOfBirth,
                    BloodType = p.BloodType != null ? p.BloodType.ToString() : null,
                    Status = p.ClinicPatients.Any(cp => cp.ClinicId == clinicId && cp.IsActive) ? "Active" : "Inactive"
                })
                .ToListAsync();

            return new PagedResponse<GetAllPatientsDtoResponse>(data, totalRecords, request.PageNumber, request.PageSize);
        }

        public async Task<GetPatientInformationForAppointmentDtoResponse?> GetPatientInformationForAppointmentAsync(PatientAppointmentSearchDtoRequest search, int clinicId)
        {
            var searcnName = search.Name.Trim().ToLower();

            return await _appDbContext.Patients.AsNoTracking()
            .Where(p => p.Person.PhoneNumber == search.PhoneNumber&&
                (
                    p.Person.FirstName.ToLower().Contains(searcnName)
                    || p.Person.LastName.ToLower().Contains(searcnName)
                    || (p.Person.FirstName + " " + p.Person.LastName)
                        .ToLower()
                        .Contains(searcnName)
                )&& p.ClinicPatients.Any(cp => cp.ClinicId == clinicId && cp.PatientId == p.Id))
            .Select(p => new GetPatientInformationForAppointmentDtoResponse
            {
                Id = p.Id,
                FullName = $"{p.Person.FirstName} {p.Person.LastName}",
                PhoneNumber = p.Person.PhoneNumber!,
                Gender = p.Gender.ToString(),
            })
            .SingleOrDefaultAsync();
                }
    }
}