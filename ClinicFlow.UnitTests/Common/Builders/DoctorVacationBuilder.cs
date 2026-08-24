using ClinicFlow.Application.Features.DoctorVacations.DTOs;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class DoctorVacationBuilder
    {
        public static Get_Create_Update_DoctorVacationDto CreateRequest(int userId, DateOnly startDate)
        {
            return new Get_Create_Update_DoctorVacationDto
            {
                Id = 10,
                UserId = userId,
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                Reason = "Annual Leave"
            };
        }

        public static DoctorVacation CreateDoctorVacation(int doctorId)
        {
            return new DoctorVacation
            {
                Id = 10,
                DoctorId = doctorId,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
                Status = DoctorVacationStatusEnum.NotStarted
            };
        }
    }
}