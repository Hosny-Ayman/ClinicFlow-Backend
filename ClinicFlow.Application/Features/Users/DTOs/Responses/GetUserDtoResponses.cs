namespace ClinicFlow.Application.Features.Users.DTOs.Responses
{
    public sealed record GetUserDtoResponses
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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
