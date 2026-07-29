namespace ClinicFlow.Domain.Entities
{
    public class ClinicWorkingHour
    {
        public int Id { get; set; }

        public int ClinicId { get; set; }

        public DayOfWeek Day { get; set; }

        public TimeOnly OpenTime { get; set; }

        public TimeOnly CloseTime { get; set; }

        public bool IsClosed { get; set; }

        public Clinic Clinic { get; set; } = null!;
       
    }

}
