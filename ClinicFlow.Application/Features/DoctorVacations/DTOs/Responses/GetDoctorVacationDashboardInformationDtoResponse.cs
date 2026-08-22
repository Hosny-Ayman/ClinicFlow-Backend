namespace ClinicFlow.Application.Features.DoctorVacations.DTOs.Responses
{
    public sealed record GetDoctorVacationDashboardInformationDtoResponse
    {

        public int TotalLeavesCount { get; set; }

        public int UpcomingLeavesCount { get; set; }

        public int ActiveLeavesCount { get; set; }

        public int CompletedLeavesCount { get; set; }

    }
}
