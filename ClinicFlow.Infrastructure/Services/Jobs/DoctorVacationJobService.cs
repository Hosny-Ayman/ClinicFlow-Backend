using ClinicFlow.Application.Common.Interfaces.Jobs;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Services.Jobs
{
    public class DoctorVacationJobService : IDoctorVacationJobService
    {
        private readonly AppDbContext _appDbContext;

        public DoctorVacationJobService(AppDbContext appDbContext)
        {
           _appDbContext = appDbContext;
        }

        public async Task UpdateExpiredVacations()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            var vacations = await _appDbContext.DoctorVacations
                .Where(x => x.EndDate < today &&
                            x.Status != DoctorVacationStatusEnum.Cancelled && x.Status != DoctorVacationStatusEnum.Completed)
                .ToListAsync();


            foreach (var vacation in vacations)
            {
                vacation.Status = DoctorVacationStatusEnum.Completed;
            }

            await _appDbContext.SaveChangesAsync();
        }
    }
}
