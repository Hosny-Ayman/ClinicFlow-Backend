using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.SysteamSettings;
using ClinicFlow.Application.Features.SysteamSettings.DTOs.Requests;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.UnitTests.Common.Mocks;
using Moq;

namespace ClinicFlow.UnitTests.SysteamSettings
{
    public class SysteamSettingServiceTests
    {
        private readonly Mock<ISysteamSettingRepository> _systeamSettingRepositoryMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly Mock<ISysteamSettingService> _systeamSettingQueryServiceMock;

        private const string ImageKey = "logo";
        private const string ImageValue = "logo.png";
        private const string FormattedUrl = "https://clinicflow.com/images/logo.png";

        public SysteamSettingServiceTests()
        {
            _systeamSettingRepositoryMock = SysteamSettingMocks.SysteamSettingRepository();
            _fileStorageServiceMock = SysteamSettingMocks.FileStorageService();
            _systeamSettingQueryServiceMock = SysteamSettingMocks.SysteamSettingService();
        }

        private SysteamSettingService CreateService()
        {
            return new SysteamSettingService(
                _systeamSettingRepositoryMock.Object,
                _fileStorageServiceMock.Object,
                _systeamSettingQueryServiceMock.Object
            );
        }

        [Fact]
        public async Task GetSystemImageAsync_WhenSettingNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();

            _systeamSettingQueryServiceMock
                .Setup(x => x.GetOnlySettingValueAsyncBySettingKeyAsync(ImageKey))
                .ReturnsAsync((string?)null);

            // Act
            var result = await service.GetSystemImageAsync(ImageKey);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);

            _fileStorageServiceMock.Verify(x => x.GetFileUrl(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetSystemImageAsync_WhenSettingFound_ShouldReturnMappedImageUrl()
        {
            // Arrange
            var service = CreateService();

            _systeamSettingQueryServiceMock
                .Setup(x => x.GetOnlySettingValueAsyncBySettingKeyAsync(ImageKey))
                .ReturnsAsync(ImageValue);

            _fileStorageServiceMock
                .Setup(x => x.GetFileUrl(ImageValue))
                .Returns(FormattedUrl);

            // Act
            var result = await service.GetSystemImageAsync(ImageKey);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(FormattedUrl, result.Data.ImageUrl);

            _systeamSettingQueryServiceMock.Verify(x => x.GetOnlySettingValueAsyncBySettingKeyAsync(ImageKey), Times.Once);
            _fileStorageServiceMock.Verify(x => x.GetFileUrl(ImageValue), Times.Once);
        }
    }
}

