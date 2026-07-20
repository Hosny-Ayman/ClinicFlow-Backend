using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class SysteamSettingRepository : ISysteamSettingRepository
    {
        private readonly AppDbContext _appDbContext;

        public SysteamSettingRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }



        public async Task<SysteamSetting?> GetSysteamSettingAsyncBySettingKeyAsync(string settingKey, bool Tracking = false)
        {
            var query = _appDbContext.SysteamSettings.AsQueryable();

            if (!Tracking)
                query = query.AsNoTracking();


            return await query.SingleOrDefaultAsync(x => x.SettingKey == settingKey);
        }
    }
}
