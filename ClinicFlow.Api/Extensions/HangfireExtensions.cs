using ClinicFlow.Application.Common.Interfaces.Jobs;
using Hangfire;

namespace ClinicFlow.Api.Extensions
{
    public static class HangfireExtensions
    {

        public static void AddHangfireJobs(this IApplicationBuilder app)
        {
            RecurringJob.AddOrUpdate<IDoctorVacationJobService>("update-Expired-vacations", x => x.UpdateExpiredVacations(), Cron.Daily);

            RecurringJob.AddOrUpdate<IDoctorVacationJobService>("update-NotStarted-vacations", x => x.UpdateNotStartedVacations(), Cron.Daily);

            RecurringJob.AddOrUpdate<IAppointmentNotificationJobService>("Appointment-Reminder", x => x.SendAppointmentReminderAsync(), "*/30 * * * *");
        }

        

    }
}
