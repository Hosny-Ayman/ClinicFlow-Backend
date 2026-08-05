namespace ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Requests
{
    public sealed record UpdateClinicWorkingHoursAndDaysDtoRequest
    {
        public int Id { get; set; }

        public DayOfWeek Day { get; set; }

        public TimeOnly OpenTime { get; set; }

        public TimeOnly CloseTime { get; set; }

        public bool IsClosed { get; set; }

        public int AppointmentDurationInMinutes { get; set; }


    }
}
