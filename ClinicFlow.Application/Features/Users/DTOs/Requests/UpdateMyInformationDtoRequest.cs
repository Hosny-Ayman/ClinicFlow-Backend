namespace ClinicFlow.Application.Features.Users.DTOs.Requests
{
    public sealed record UpdateMyInformationDtoRequest
    {
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string PhoneNumber { get; init; } = null!;
    }
}
