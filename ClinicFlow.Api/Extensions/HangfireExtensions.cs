using ClinicFlow.Application.Common.Interfaces.Jobs;
using Hangfire;

namespace ClinicFlow.Api.Extensions
{
    public static class HangfireExtensions
    {

        public static void AddHangfireJobs(this IApplicationBuilder app)
        {
            RecurringJob.AddOrUpdate<IDoctorVacationJobService>("update-vacations", x => x.UpdateExpiredVacations(), Cron.Daily);
        }

    }
}
