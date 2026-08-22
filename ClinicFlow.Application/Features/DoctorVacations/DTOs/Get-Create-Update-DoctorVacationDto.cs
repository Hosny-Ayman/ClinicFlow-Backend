using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.DoctorVacations.DTOs
{
    public sealed class Get_Create_Update_DoctorVacationDto
    {
        public int? Id { get; set; }

        public int UserId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string? Reason { get; set; }

        public DoctorVacationStatusEnum Status { get; set; } = DoctorVacationStatusEnum.InProgress;
    }
}
