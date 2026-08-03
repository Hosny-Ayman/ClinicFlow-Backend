using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Infrastructure.Data;

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
    }
}
