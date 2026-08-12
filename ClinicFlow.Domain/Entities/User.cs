using  ClinicFlow.Domain.Enums;

namespace ClinicFlow.Domain.Entities
{
    public class User
    {

        public int Id { get; set; }

        public int PersonId { get; set; }

        public int ClinicId { get; set; }

        public string PasswordHash { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Person Person { get; set; } = null!;

        public Clinic Clinic { get; set; } = null!;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public Doctor? Doctor { get; set; }

        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
