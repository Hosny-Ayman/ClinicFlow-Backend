using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.QueryServices
{
    public class SysteamSettingService : ISysteamSettingService
    {
        private readonly AppDbContext _appDbContext;
        public SysteamSettingService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public async Task<string?> GetOnlySettingValueAsyncBySettingKeyAsync(string SettingKey)
        {
            return await _appDbContext.SysteamSettings.Where(x => x.SettingKey == SettingKey).Select(x => x.SettingValue).FirstOrDefaultAsync();
        }
    }
}
