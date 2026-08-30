using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.Appointments.DTOs.Requests
{
    public sealed record AppointmentSearchDtoRequest
    {

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public string? SortField { get; set; }

        public int? SortOrder { get; set; }

        public string? FullNameOrPhoneNumberSearch { get; set; }

        public AppointmentStatusEnum? StatusSearch { get; set; }

        public int? DoctorIdSearch { get; set; }

        public DateOnly? DateSearch { get; set; }

    }
}
