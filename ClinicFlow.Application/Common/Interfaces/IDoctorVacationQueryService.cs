using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Features.DoctorVacations.DTOs.Requests;
using ClinicFlow.Application.Features.DoctorVacations.DTOs.Responses;

namespace ClinicFlow.Application.Common.Interfaces
{
    public interface IDoctorVacationQueryService
    {

        Task<PagedResponse<GetAllDoctorVacationInformationDtoResponse>> GetAllDoctorVacationInformationAsync(DoctorVacationSearchDtoRequest request, int clinicId);

        Task<GetDoctorVacationDashboardInformationDtoResponse> GetDoctorVacationDashboardInformationAsync(int clinicId);

    }
}
