using ClinicFlow.Application.Features.SysteamSettings.DTOs.Requests;

namespace ClinicFlow.Application.Common.Interfaces
{
    public interface ISysteamSettingService
    {
        Task<string?> GetOnlySettingValueAsyncBySettingKeyAsync(string SettingKey);
    }
}
