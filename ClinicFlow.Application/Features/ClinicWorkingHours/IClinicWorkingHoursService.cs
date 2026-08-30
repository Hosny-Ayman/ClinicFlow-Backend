using ClinicFlow.Application.Common.DTOs;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Requests;
using ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Responses;

namespace ClinicFlow.Application.Features.ClinicWorkingHours
{
    public interface IClinicWorkingHoursService
    {

        Task<OperationResult<bool>> CreateWorkingHoursAndDaysAsync(
           List<CreateClinicWorkingHourDtoRequest> request);

        Task<OperationResult<List<GetAllWorkingHoursAndDaysDtoResponse?>>>
            GetAllWorkingHoursAndDaysAsync();

        Task<OperationResult<bool>> UpdateWorkingHoursAndDaysAsync(
            List<UpdateClinicWorkingHoursAndDaysDtoRequest> request);

        Task<bool> IsTheClinicOpenAtThisAppointmentInsideProject(
            Bookappointment appointment);

    }
}
