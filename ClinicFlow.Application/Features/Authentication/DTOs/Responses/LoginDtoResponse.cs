using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.Authentication.DTOs.Responses
{
    public sealed record LoginDtoResponse
    {

        public int UserId { get; init; }

        public string FullName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public int ClinicId { get; init; }

        public List <RoleEnum> Roles { get; init; } = new List<RoleEnum>();


    }
}
