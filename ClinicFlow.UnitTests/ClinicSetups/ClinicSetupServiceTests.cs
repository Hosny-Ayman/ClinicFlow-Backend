using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.ClinicSetups;
using ClinicFlow.Application.Features.ClinicSetups.DTOs.Requests;
using ClinicFlow.Application.Features.ClinicSetups.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Moq;

namespace ClinicFlow.UnitTests.ClinicSetups
{
    public class ClinicSetupServiceTests
    {
        private readonly Mock<IClinicSetupRepository> _clinicSetupRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IClinicSetupQueryService> _clinicSetupQueryServiceMock;

        private const int ClinicId = 10;
        private const int SetupId = 100;

        public ClinicSetupServiceTests()
        {
            _clinicSetupRepositoryMock = ClinicMocks.ClinicSetupRepository();
            _unitOfWorkMock = CommonMocks.UnitOfWork();
            _currentUserServiceMock = CommonMocks.CurrentUserService();
            _clinicSetupQueryServiceMock = ClinicMocks.ClinicSetupQueryService();

            _currentUserServiceMock.Setup(x => x.ClinicId).Returns(ClinicId);

            var config = new MapperConfiguration(cfg =>
            {
                // Register any required profiles here, e.g., ClinicSetupProfile if it exists.
                // Or just test using Moq for Mapper if profiles are not strictly needed, 
                // but standard practice in this suite is to load the actual mapping assembly.
                cfg.AddMaps(typeof(ClinicSetupService).Assembly);
            });
            _mapper = config.CreateMapper();
        }

        private ClinicSetupService CreateService()
        {
            return new ClinicSetupService(
                _clinicSetupRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _mapper,
                _currentUserServiceMock.Object,
                _clinicSetupQueryServiceMock.Object
            );
        }

        #region CreateClinicSetupAsync

        [Fact]
        public async Task CreateClinicSetupAsync_WhenSetupAlreadyExists_ShouldReturnSuccessZero()
        {
            // Arrange
            var service = CreateService();
            var request = ClinicSetupBuilder.CreateAndEditClinicSetupDto();

            _clinicSetupRepositoryMock
                .Setup(x => x.IsClinicSetupExistsAsync(ClinicId))
                .ReturnsAsync(true);

            // Act
            var result = await service.CreateClinicSetupAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Data);

            _clinicSetupRepositoryMock.Verify(x => x.AddClinicSetupStatusAsync(It.IsAny<ClinicSetup>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateClinicSetupAsync_WhenSetupDoesNotExist_ShouldCreateAndReturnId()
        {
            // Arrange
            var service = CreateService();
            var request = ClinicSetupBuilder.CreateAndEditClinicSetupDto(hasSkippedSetup: true);

            _clinicSetupRepositoryMock
                .Setup(x => x.IsClinicSetupExistsAsync(ClinicId))
                .ReturnsAsync(false);

            _clinicSetupRepositoryMock
                .Setup(x => x.AddClinicSetupStatusAsync(It.IsAny<ClinicSetup>()))
                .Callback<ClinicSetup>(setup => setup.Id = SetupId)
                .ReturnsAsync(SetupId);

            // Act
            var result = await service.CreateClinicSetupAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(SetupId, result.Data);

            _clinicSetupRepositoryMock.Verify(x => x.AddClinicSetupStatusAsync(It.Is<ClinicSetup>(s => s.ClinicId == ClinicId && s.HasSkippedSetup == true)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region UpdateClinicSetupAsync

        [Fact]
        public async Task UpdateClinicSetupAsync_WhenSetupNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            var request = ClinicSetupBuilder.CreateAndEditClinicSetupDto();

            _clinicSetupRepositoryMock
                .Setup(x => x.GetClinicSetupAsync(ClinicId, true))
                .ReturnsAsync((ClinicSetup?)null);

            // Act
            var result = await service.UpdateClinicSetupAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateClinicSetupAsync_WhenSetupFound_ShouldUpdateAndReturnSuccess()
        {
            // Arrange
            var service = CreateService();
            var request = ClinicSetupBuilder.CreateAndEditClinicSetupDto(hasSkippedSetup: true);
            var existingSetup = ClinicSetupBuilder.CreateClinicSetupEntity(id: SetupId, clinicId: ClinicId, hasSkippedSetup: false);

            _clinicSetupRepositoryMock
                .Setup(x => x.GetClinicSetupAsync(ClinicId, true))
                .ReturnsAsync(existingSetup);

            // Act
            var result = await service.UpdateClinicSetupAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);

            Assert.True(existingSetup.HasSkippedSetup); // Verify mapper applied the change
            Assert.Equal(ClinicId, existingSetup.ClinicId); // Verify ClinicId assignment

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region GetClinicSetupStatusAsync

        [Fact]
        public async Task GetClinicSetupStatusAsync_WhenStatusQueryReturnsNull_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();

            _clinicSetupQueryServiceMock
                .Setup(x => x.GetClinicSetupStatusAsync(ClinicId, false))
                .ReturnsAsync((GetClinicSetupStatusDtoResponse?)null);

            // Act
            var result = await service.GetClinicSetupStatusAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetClinicSetupStatusAsync_WhenStatusQueryReturnsData_ShouldReturnMappedResponse()
        {
            // Arrange
            var service = CreateService();
            var expectedResponse = ClinicSetupBuilder.GetClinicSetupStatusDtoResponse();

            _clinicSetupQueryServiceMock
                .Setup(x => x.GetClinicSetupStatusAsync(ClinicId, false))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await service.GetClinicSetupStatusAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            
            // The service maps the response to itself essentially since GetClinicSetupStatusAsync returns the DTO response directly.
            // Wait, the query service returns GetClinicSetupStatusDtoResponse, and the service maps it to GetClinicSetupStatusDtoResponse again?
            // Yes, var steupDto = _mapper.Map<GetClinicSetupStatusDtoResponse>(steup);
            Assert.Equal(expectedResponse.Progress, result.Data.Progress);
            Assert.Equal(expectedResponse.IsSetupCompleted, result.Data.IsSetupCompleted);
        }

        #endregion
    }
}

