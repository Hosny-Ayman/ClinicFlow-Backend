using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.DoctorVacations.DTOs;
using ClinicFlow.Application.Features.DoctorVacations.DTOs.Requests;
using ClinicFlow.Application.Features.DoctorVacations.DTOs.Responses;

namespace ClinicFlow.Application.Features.DoctorVacations
{
    public interface IDoctorVacationService
    {
        Task<OperationResult<int>> CreateDoctorVacationAsyn(
            Get_Create_Update_DoctorVacationDto request);

        Task<OperationResult<bool>> UpdateDoctorVacationAsyn(
            Get_Create_Update_DoctorVacationDto request);

        Task<OperationResult<Get_Create_Update_DoctorVacationDto>>
            GetDoctorVacationInformationAsync(
                int userId,
                int vacationId);

        Task<OperationResult<PagedResponse<GetAllDoctorVacationInformationDtoResponse>>>
            GetAllDoctorVacationInformationAsync(
                DoctorVacationSearchDtoRequest request);

        Task<OperationResult<GetDoctorVacationDashboardInformationDtoResponse>>
            GetDoctorVacationDashboardInformationAsync();

        Task<bool> HasDoctorVacationOnDate(
            DateOnly appointmentDate,
            int doctorId);
    }
}