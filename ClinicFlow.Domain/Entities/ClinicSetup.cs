namespace ClinicFlow.Domain.Entities
{
    public class ClinicSetup
    {
        public int Id { get; set; }

        public int ClinicId { get; set; }

        public bool IsSetupCompleted { get; set; }

        public bool HasSkippedSetup { get; set; }

        public Clinic clinic { get; set; } = null!;
    }

}
