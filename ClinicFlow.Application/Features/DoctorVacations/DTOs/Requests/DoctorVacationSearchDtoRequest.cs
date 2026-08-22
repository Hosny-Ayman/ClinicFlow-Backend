using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.DoctorVacations.DTOs.Requests
{
    public sealed record DoctorVacationSearchDtoRequest
    {

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public string? SortField { get; set; }

        public int? SortOrder { get; set; }

        public string? FullNameSearch { get; set; }

        public string? EmailSearch { get; set; }

        public string? PhoneNumberSearch { get; set; }

        public GenderEnum? Gender { get; set; }

        public int? SpecialtyId { get; set; }

        public DoctorVacationStatusEnum? Status { get; set; }

        public DateOnly? From { get; set; }

        public DateOnly? To { get; set; }

    }
}
