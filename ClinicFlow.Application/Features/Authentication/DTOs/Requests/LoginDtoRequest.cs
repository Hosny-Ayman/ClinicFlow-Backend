namespace ClinicFlow.Application.Features.Authentication.DTOs.Requests
{
    public sealed record LoginDtoRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

    }
}
