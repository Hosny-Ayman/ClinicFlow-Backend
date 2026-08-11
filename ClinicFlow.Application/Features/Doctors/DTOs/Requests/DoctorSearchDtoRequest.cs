using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.Doctors.DTOs.Requests
{
    public sealed record DoctorSearchDtoRequest
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

    }
}
