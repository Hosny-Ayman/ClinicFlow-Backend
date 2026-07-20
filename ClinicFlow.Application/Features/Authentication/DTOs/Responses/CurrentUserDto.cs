namespace ClinicFlow.Application.Features.Authentication.DTOs.Responses
{
    public sealed record CurrentUserDto
    {

        public int Id { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public int ClinicId { get; init; } 
        public IReadOnlyList<string> Roles { get; init; } = [];

    }
}
