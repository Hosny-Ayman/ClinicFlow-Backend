using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Specifications;
using ClinicFlow.Application.Features.Appointments.DTOs.Requests;
using ClinicFlow.Application.Features.Appointments.DTOs.Responses;
using ClinicFlow.Application.Features.Appointments.DTOs.Specifications;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.QueryServices
{
    public class AppointmentQueryService : IAppointmentQueryService
    {
        private readonly AppDbContext _appDbContext;
        public AppointmentQueryService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<PagedResponse<GetAllAppointmentDtoResponse>> GetAllAppointmentAsync(AppointmentSearchDtoRequest request, int clinicId)
        {

            var query = _appDbContext.Appointments.AsNoTracking().AsQueryable();

            var spec = new AppointmentWithSearchSpecification(request, clinicId);

            query = SpecificationEvaluator.GetQuery(query, spec);

            var totalrecords = await query.CountAsync();

            var data = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new GetAllAppointmentDtoResponse
                {
                    AppointmentId = x.Id,
                    Time = x.StartTime,
                    PatientFullName = $"{x.Patient.Person.FirstName} {x.Patient.Person.LastName}",
                    DoctorFullName = $"{x.Doctor.User.Person.FirstName} {x.Doctor.User.Person.LastName}",
                    PatientPhoneNumber = x.Patient.Person.PhoneNumber ?? "",
                    DoctorSpecialtie = x.Doctor.Specialty.Name,
                    Status = x.Status.ToString(),
                    ConsultationFee = x.Doctor.ConsultationFee,
                    Paymentstatus = x.Invoice.Status.ToString(),

                }).ToListAsync();
                

            return new PagedResponse<GetAllAppointmentDtoResponse>(data, totalrecords, request.PageNumber, request.PageSize);
        }

        public async Task<GetAppointmentDashboardDtoResponse> GetAppointmentDashboardAsync(DateOnly date,int clinicId)
        {
            return await _appDbContext.Appointments
                .Where(x => x.ClinicId == clinicId && x.AppointmentDate == date)
                .GroupBy(x => 1)
                .Select(d => new GetAppointmentDashboardDtoResponse
                {
                    TotalAppointments = d.Count(),

                    TodayAppointments = d.Count(x =>
                        (x.Status == AppointmentStatusEnum.CheckedIn || x.Status == AppointmentStatusEnum.Completed || x.Status == AppointmentStatusEnum.InProgress)
                        && x.AppointmentDate == DateOnly.FromDateTime(DateTime.Now)),

                    PendingAppointments = d.Count(x =>
                        x.Status == AppointmentStatusEnum.CheckedIn),

                    CancelledAppointments = d.Count(x =>
                        x.Status == AppointmentStatusEnum.Cancelled)
                }).FirstOrDefaultAsync() ?? new GetAppointmentDashboardDtoResponse();
        }
    }
}
