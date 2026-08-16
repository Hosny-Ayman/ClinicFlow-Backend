using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.Interfaces
{
    public interface ISysteamSettingRepository
    {

        Task<SysteamSetting?> GetSysteamSettingAsyncBySettingKeyAsync(string settingKey,bool Tracking = false);

    }
}
