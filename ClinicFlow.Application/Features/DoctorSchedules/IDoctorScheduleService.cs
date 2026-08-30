using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.DoctorSchedules.DTOs.Requests;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.DoctorSchedules
{
    public interface IDoctorScheduleService
    {
        Task AddDoctorSchedulesInsideProjectAsync(Doctor doctor);

        Task<OperationResult<List<UpdateAndGetDoctorScheduleDtoRequest>>>
            GetAllDoctorSchedulesAsync(int userId);

        Task<OperationResult<bool>>
            UpdateSchedulesInsideProjectAsync(
                List<UpdateAndGetDoctorScheduleDtoRequest> request,
                int userId);

        Task<bool>
            IsDoctorScheduleAvailableAsync(
                DayOfWeek day,
                int doctorId,
                TimeOnly appointmentTime);
    }
}