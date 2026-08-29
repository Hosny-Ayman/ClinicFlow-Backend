namespace ClinicFlow.Application.Common.DTOs
{
    public sealed record Bookappointment
    {

        public DayOfWeek Day { get; set; }

        public TimeOnly StartTime { get; set; }

    }
}
