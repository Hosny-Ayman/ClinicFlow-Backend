using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.Patients.DTOs.Requests
{
    public sealed record PatientSearchDtoRequest
    {

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public string? SortField { get; set; }

        public int? SortOrder { get; set; }

        public string? FullNameSearch { get; set; }

        public string? PhoneNumberSearch { get; set; }

        public string? NationalIdSearch { get; set; }

        public GenderEnum? Gender { get; set; }

        public BloodTypeEnum? BloodType { get; set; }

    }
}
