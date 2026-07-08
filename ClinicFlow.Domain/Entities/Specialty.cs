namespace ClinicFlow.Domain.Entities
{
    public class Specialty
    {

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    }
}
