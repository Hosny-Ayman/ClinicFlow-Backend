namespace ClinicFlow.Domain.Entities
{
    public class SysteamSetting
    {
        public int Id { get; set; }

        public string SettingKey { get; set; } = string.Empty;

        public string SettingValue { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

    }


}
