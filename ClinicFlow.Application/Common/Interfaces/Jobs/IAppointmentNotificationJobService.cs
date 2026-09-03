namespace ClinicFlow.Application.Common.Interfaces.Jobs
{
    public interface IAppointmentNotificationJobService
    {

        Task SendAppointmentReminderAsync();
    }
}
