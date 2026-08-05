namespace ClinicFlow.Application.Features.Users.DTOs.Requests
{
    public sealed record UpdateUserInformationDtoRequest
    {

        public int Id { get; init; }
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string? Password { get; init; }
        public string PhoneNumber { get; init; } = null!;
        public bool IsActive { get; init; } = true;
     

    }
}
