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
        private readonly IFileStorageService _fileStorageService;
        public AppointmentQueryService(AppDbContext appDbContext, IFileStorageService fileStorageService)
        {
            _appDbContext = appDbContext;
            _fileStorageService = fileStorageService;
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

        public async Task<GetAppointmentDashboardDtoResponse> GetDoctorAppointmentDashboardAsync(int doctorId, DateOnly date, int clinicId)
        {
            return await _appDbContext.Appointments
                .Where(x => x.ClinicId == clinicId && x.DoctorId == doctorId && x.AppointmentDate == date)
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

        public async Task<GetAdminDashboardStatisticsDtoResponse> GetAdminDashboardStatisticsAsync(int clinicId, DateOnly today)
        {
            var baseCounters = await GetAppointmentDashboardAsync(today, clinicId);

            var yesterday = today.AddDays(-1);
            var yesterdayCounters = await GetAppointmentDashboardAsync(yesterday, clinicId);

            var response = new GetAdminDashboardStatisticsDtoResponse
            {
                TotalAppointments = baseCounters.TotalAppointments,
                TotalAppointmentsDiff = baseCounters.TotalAppointments - yesterdayCounters.TotalAppointments,

                AttendedAppointments = baseCounters.TodayAppointments,
                AttendedAppointmentsDiff = baseCounters.TodayAppointments - yesterdayCounters.TodayAppointments,

                WaitingAppointments = baseCounters.PendingAppointments,
                WaitingAppointmentsDiff = baseCounters.PendingAppointments - yesterdayCounters.PendingAppointments,

                CancelledAppointments = baseCounters.CancelledAppointments,
                CancelledAppointmentsDiff = baseCounters.CancelledAppointments - yesterdayCounters.CancelledAppointments
            };

            var statusGroups = await _appDbContext.Appointments
                .Where(x => x.ClinicId == clinicId && x.AppointmentDate == today)
                .GroupBy(x => x.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            response.AppointmentsByStatus = statusGroups.Select(g => new AppointmentStatusBreakdownDto(
                g.Status.ToString(),
                g.Count
            )).ToList();

            var timeGroups = await _appDbContext.Appointments
                .Where(x => x.ClinicId == clinicId && x.AppointmentDate == today)
                .GroupBy(x => x.StartTime.Hour)
                .Select(g => new { Hour = g.Key, Count = g.Count() })
                .ToListAsync();

            response.AppointmentsByTimePeriod = timeGroups
                .GroupBy(g => (g.Hour / 2) * 2)
                .Select(bucket =>
                {
                    int start = bucket.Key;
                    int end = start + 2;

                    int displayStart = start > 12 ? start - 12 : (start == 0 ? 12 : start);
                    int displayEnd = end > 12 ? end - 12 : (end == 0 ? 12 : end);
                    string amPm = start >= 12 && start < 24 ? "م" : "ص";

                    string period = $"{displayStart}-{displayEnd} {amPm}";
                    return new AppointmentTimeBreakdownDto(period, bucket.Sum(x => x.Count));
                })
                .ToList();

            var topDoctorsQuery = await _appDbContext.Appointments
                .Where(x => x.ClinicId == clinicId)
                .GroupBy(x => new
                {
                    x.DoctorId,
                    x.Doctor.User.Person.FirstName,
                    x.Doctor.User.Person.LastName,
                    SpecialtyName = x.Doctor.Specialty.Name,
                    x.Doctor.ProfileImageUrl
                })
                .Select(g => new
                {
                    g.Key.FirstName,
                    g.Key.LastName,
                    g.Key.SpecialtyName,
                    g.Key.ProfileImageUrl,
                    AppointmentCount = g.Count()
                })
                .OrderByDescending(x => x.AppointmentCount)
                .Take(5)
                .ToListAsync();

                response.TopDoctors = topDoctorsQuery.Select(x => new TopDoctorDto
                {
                 DoctorName = $"{x.FirstName} {x.LastName}",
                 Specialty = x.SpecialtyName,
                 ImageUrl = x.ProfileImageUrl,
                 AppointmentCount = x.AppointmentCount
                }).ToList();

            foreach (var item in response.TopDoctors)
            {
                item.ImageUrl = item.ImageUrl is not null ? _fileStorageService.GetFileUrl(item.ImageUrl) : null;
            }

            return response;
        }
    }
}
