using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class ClinicWorkingHourRepository : IClinicWorkingHourRepository
    {
        private readonly AppDbContext _appDbContext;
        public ClinicWorkingHourRepository(AppDbContext appDbContext)
        {

            _appDbContext = appDbContext;
        }

        public async Task AddWorkingHoursAndDaysAsync(List<ClinicWorkingHour> Days)
        {
            await _appDbContext.AddRangeAsync(Days);
        }

        public async Task<List<ClinicWorkingHour>> GetAllWorkingHoursAndDaysAsync(int clinicId, bool tracking = false)
        {
            var query = _appDbContext.ClinicWorkingHours.AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();


            return await query.Where(x => x.ClinicId == clinicId).ToListAsync();
        }
    }
}
