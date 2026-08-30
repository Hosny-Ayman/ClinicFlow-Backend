using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Features.Appointments.DTOs.Requests;
using ClinicFlow.Application.Features.Appointments.DTOs.Responses;

namespace ClinicFlow.Application.Common.Interfaces
{
    public interface IAppointmentQueryService
    {

        Task<PagedResponse<GetAllAppointmentDtoResponse>> GetAllAppointmentAsync (AppointmentSearchDtoRequest request ,int clinicId);

        Task<GetAppointmentDashboardDtoResponse> GetAppointmentDashboardAsync(DateOnly date, int clinicId);

    }
}
