using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Domain.Interfaces;
using Moq;

namespace ClinicFlow.UnitTests.Common.Mocks
{
    public static class SysteamSettingMocks
    {
        public static Mock<ISysteamSettingRepository> SysteamSettingRepository() => new Mock<ISysteamSettingRepository>();

        public static Mock<ISysteamSettingService> SysteamSettingService() => new Mock<ISysteamSettingService>();

        public static Mock<IFileStorageService> FileStorageService() => new Mock<IFileStorageService>();
    }
}

