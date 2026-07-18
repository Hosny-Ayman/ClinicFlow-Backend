namespace ClinicFlow.Domain.Entities
{
    public class Clinic
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? LogoUrl { get; set; }

        public string Phone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<User> Users { get; set; } = new List<User>();

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

        public ICollection<Patient> Patients { get; set; } = new List<Patient>();

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();



    }
}
