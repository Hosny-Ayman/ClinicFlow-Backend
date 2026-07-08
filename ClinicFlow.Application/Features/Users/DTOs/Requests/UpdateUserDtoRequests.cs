namespace ClinicFlow.Application.Features.Users.DTOs.Requests
{
    public sealed record UpdateUserDtoRequests
    {

        public int Id { get; set; }

        public int ClinicId { get; set; }

        public int RoleId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;


        public bool IsActive { get; set; } = true;

    }
}
