using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Features.Appointments.DTOs.Requests;
using ClinicFlow.Application.Features.Appointments.DTOs.Responses;
using ClinicFlow.Application.Features.Patients.DTOs;

namespace ClinicFlow.Application.Common.Interfaces
{
    public interface IAppointmentQueryService
    {

        Task<PagedResponse<GetAllAppointmentDtoResponse>> GetAllAppointmentAsync (AppointmentSearchDtoRequest request ,int clinicId);

        Task<GetAppointmentDashboardDtoResponse> GetAppointmentDashboardAsync(DateOnly date, int clinicId);

        Task<GetAppointmentDashboardDtoResponse> GetDoctorAppointmentDashboardAsync(int doctorId, DateOnly date, int clinicId);

        Task<GetAdminDashboardStatisticsDtoResponse> GetAdminDashboardStatisticsAsync(int clinicId, DateOnly today);

        Task<List<NotificationAppointmentPatientInfoDto>> GetAllCloseToStartAppointments();

        Task SendAppointmentBookedAsync(NotificationAppointmentPatientInfoDto appointment);

        Task SendAppointmentCancelledAsync(NotificationAppointmentPatientInfoDto appointment);

    }
}
