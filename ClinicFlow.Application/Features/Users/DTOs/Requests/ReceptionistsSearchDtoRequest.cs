using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.Users.DTOs.Requests
{
    public sealed record ReceptionistsSearchDtoRequest
    {

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public string? SortField { get; set; }

        public int? SortOrder { get; set; }

        public string? FullNameSearch { get; set; }

        public string? EmailSearch { get; set; }

        public string? PhoneNumberSearch { get; set; }

        public bool? Status { get; set; }


    }
}
