using System.Reflection;

namespace ClinicFlow.Domain.Entities
{
    public class DoctorVacation
    {

        public int Id { get; set; }

        public int DoctorId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string? Reason { get; set; }

        public Doctor Doctor { get; set; } = null!;
    }
}
