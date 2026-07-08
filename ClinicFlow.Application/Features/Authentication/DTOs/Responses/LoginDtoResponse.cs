using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Application.Features.Authentication.DTOs.Responses
{
    public sealed record LoginDtoResponse
    {

        public int UserId { get; init; }

        public string FullName { get; init; } = null!;

        public string Email { get; init; } = null!;

        public RoleEnum Role { get; init; }


    }
}
