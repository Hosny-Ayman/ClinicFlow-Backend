namespace ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Requests
{
    public sealed record CreateClinicWorkingHourDtoRequest
    {
       

        public DayOfWeek Day { get; init; }

        public TimeOnly OpenTime { get; init; }

        public TimeOnly CloseTime { get; init; }

        public bool IsClosed { get; init; }

        public int AppointmentDurationInMinutes { get; init; }


    }
}
