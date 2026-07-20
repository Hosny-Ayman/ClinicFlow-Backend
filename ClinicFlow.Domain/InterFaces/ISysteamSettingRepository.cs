using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.InterFaces
{
    public interface ISysteamSettingRepository
    {

        Task<SysteamSetting?> GetSysteamSettingAsyncBySettingKeyAsync(string settingKey,bool Tracking = false);

    }
}
