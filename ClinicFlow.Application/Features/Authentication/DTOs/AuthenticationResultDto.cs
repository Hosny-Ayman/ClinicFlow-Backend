using ClinicFlow.Application.Features.Authentication.DTOs.Responses;

namespace ClinicFlow.Application.Features.Authentication.DTOs
{
    public sealed record AuthenticationResultDto
    {
        public string AccessToken { get; init; } = string.Empty;

        public string RefreshToken { get; init; } = string.Empty;

        public LoginDtoResponse User { get; init; } = default!;
    }
}
