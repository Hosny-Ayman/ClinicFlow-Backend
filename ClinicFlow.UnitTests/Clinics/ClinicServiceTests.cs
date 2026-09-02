using AutoMapper;
using ClinicFlow.Application.Common.Security;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Clinics;
using ClinicFlow.Application.Features.Clinics.DTOs.Requests;
using ClinicFlow.Application.Features.Clinics.DTOs.Responses;
using ClinicFlow.Application.Features.Users;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Moq;

namespace ClinicFlow.UnitTests.Clinics
{
    public class ClinicServiceTests
    {
        private readonly Mock<IClinicRepository> _clinicRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IMapper _mapper;
        private readonly Mock<IOwnershipService> _ownershipServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IClinicQueryService> _clinicQueryServiceMock;
        private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly Mock<IClinicSetupRepository> _clinicSetupRepositoryMock;

        // Dependencies for the concrete UserService we must inject
        private readonly Mock<IUserQueryService> _userQueryServiceMock;
        private readonly Mock<ICheckService> _authorizationServiceMock;

        private const int ClinicId = 10;
        private const string UploadedImageId = "uploaded-image-id";

        public ClinicServiceTests()
        {
            _clinicRepositoryMock = ClinicMocks.ClinicRepository();
            _userRepositoryMock = AuthenticationMocks.UserRepository();
            _ownershipServiceMock = ClinicMocks.OwnershipService();
            _unitOfWorkMock = CommonMocks.UnitOfWork();
            _clinicQueryServiceMock = ClinicMocks.ClinicQueryService();
            _userRoleRepositoryMock = DoctorMocks.UserRoleRepository();
            _currentUserServiceMock = CommonMocks.CurrentUserService();
            _fileStorageServiceMock = DoctorMocks.FileStorageService();
            _clinicSetupRepositoryMock = ClinicMocks.ClinicSetupRepository();

            _userQueryServiceMock = UserQueryMocks.UserQueryService();
            _authorizationServiceMock = DoctorMocks.CheckService();

            _currentUserServiceMock.Setup(x => x.ClinicId).Returns(ClinicId);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(ClinicProfile).Assembly);
            });
            _mapper = config.CreateMapper();
        }

        private ClinicService CreateService()
        {
            var userService = new UserService(
                _userQueryServiceMock.Object,
                _currentUserServiceMock.Object,
                _userRepositoryMock.Object,
                _mapper, // Using the same mapper, though it doesn't matter for CreateUserWithoutClinicId
                _userRoleRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _authorizationServiceMock.Object
            );

            return new ClinicService(
                _clinicRepositoryMock.Object,
                _userRepositoryMock.Object,
                _mapper,
                _ownershipServiceMock.Object,
                _unitOfWorkMock.Object,
                _clinicQueryServiceMock.Object,
                _userRoleRepositoryMock.Object,
                userService,
                _currentUserServiceMock.Object,
                _fileStorageServiceMock.Object,
                _clinicSetupRepositoryMock.Object
            );
        }

        #region CreateClinicAsync

        [Fact]
        public async Task CreateClinicAsync_WhenUserCreationFails_ShouldReturnBadRequest()
        {
            // Arrange
            var service = CreateService();
            var clinicDto = ClinicBuilder.CreateAndEditClinicDto();
            var userDto = UserBuilder.CreateAndEditUserDto();

            // Simulate user creation failure (e.g., email exists)
            _userRepositoryMock.Setup(x => x.IsEmailExitsAsync(userDto.Email)).ReturnsAsync(true);

            // Act
            var result = await service.CreateClinicAsync(clinicDto, userDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);

            _fileStorageServiceMock.Verify(x => x.UploadImageAsync(It.IsAny<Microsoft.AspNetCore.Http.IFormFile>()), Times.Never);
            _clinicRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Clinic>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateClinicAsync_WhenQueryServiceReturnsNull_ShouldReturnFailure()
        {
            // Arrange
            var service = CreateService();
            var clinicDto = ClinicBuilder.CreateAndEditClinicDto(includeLogo: true);
            var userDto = UserBuilder.CreateAndEditUserDto();

            _userRepositoryMock.Setup(x => x.IsEmailExitsAsync(userDto.Email)).ReturnsAsync(false);
            _userRepositoryMock.Setup(x => x.IsPhoneExitsAsync(userDto.PhoneNumber)).ReturnsAsync(false);

            _fileStorageServiceMock
                .Setup(x => x.UploadImageAsync(clinicDto.LogoUrl!))
                .ReturnsAsync(UploadedImageId);

            _clinicQueryServiceMock
                .Setup(x => x.GetClinicInfoWithOwnerFullnameAsync(It.IsAny<int>()))
                .ReturnsAsync((CreateClinicResponse?)null);

            // Act
            var result = await service.CreateClinicAsync(clinicDto, userDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.ServerError, result.Status); // Failure uses ServerError
            
            _fileStorageServiceMock.Verify(x => x.UploadImageAsync(clinicDto.LogoUrl!), Times.Once);
            _clinicRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Clinic>()), Times.Once);
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
            _userRoleRepositoryMock.Verify(x => x.AssignRoleAsync(It.IsAny<User>(), RoleEnum.ClinicOwner), Times.Once);
            _clinicSetupRepositoryMock.Verify(x => x.AddClinicSetupStatusAsync(It.IsAny<ClinicSetup>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateClinicAsync_WhenSuccessful_ShouldReturnCreateClinicResponse()
        {
            // Arrange
            var service = CreateService();
            var clinicDto = ClinicBuilder.CreateAndEditClinicDto(includeLogo: false);
            var userDto = UserBuilder.CreateAndEditUserDto();
            var expectedResponse = ClinicBuilder.CreateClinicResponse();

            _userRepositoryMock.Setup(x => x.IsEmailExitsAsync(userDto.Email)).ReturnsAsync(false);
            _userRepositoryMock.Setup(x => x.IsPhoneExitsAsync(userDto.PhoneNumber)).ReturnsAsync(false);

            _clinicQueryServiceMock
                .Setup(x => x.GetClinicInfoWithOwnerFullnameAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await service.CreateClinicAsync(clinicDto, userDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Same(expectedResponse, result.Data);

            _fileStorageServiceMock.Verify(x => x.UploadImageAsync(It.IsAny<Microsoft.AspNetCore.Http.IFormFile>()), Times.Never);
            _clinicRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Clinic>()), Times.Once);
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region UpdateClinicAsync

        [Fact]
        public async Task UpdateClinicAsync_WhenClinicNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            var request = ClinicBuilder.CreateAndEditClinicDto();

            _clinicRepositoryMock
                .Setup(x => x.GetClinicByIdAsync(ClinicId, true))
                .ReturnsAsync((Clinic?)null);

            // Act
            var result = await service.UpdateClinicAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateClinicAsync_WhenLogoProvided_ShouldUploadImageAndSave()
        {
            // Arrange
            var service = CreateService();
            var request = ClinicBuilder.CreateAndEditClinicDto(includeLogo: true);
            var clinic = ClinicBuilder.CreateClinicEntity(id: ClinicId, logoUrl: "old-image");

            _clinicRepositoryMock
                .Setup(x => x.GetClinicByIdAsync(ClinicId, true))
                .ReturnsAsync(clinic);

            _fileStorageServiceMock
                .Setup(x => x.UploadImageAsync(request.LogoUrl!))
                .ReturnsAsync(UploadedImageId);

            // Act
            var result = await service.UpdateClinicAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(UploadedImageId, clinic.LogoUrl);
            _fileStorageServiceMock.Verify(x => x.UploadImageAsync(request.LogoUrl!), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _fileStorageServiceMock.Verify(x => x.DeleteImageAsync(It.IsAny<string>()), Times.Never); // IsImageDelted is false
        }

        [Fact]
        public async Task UpdateClinicAsync_WhenLogoIsNullAndIsImageDeletedIsTrue_ShouldDeleteOldImage()
        {
            // Arrange
            var service = CreateService();
            var request = ClinicBuilder.CreateAndEditClinicDto(includeLogo: false, isImageDeleted: true);
            var clinic = ClinicBuilder.CreateClinicEntity(id: ClinicId, logoUrl: "old-image");

            _clinicRepositoryMock
                .Setup(x => x.GetClinicByIdAsync(ClinicId, true))
                .ReturnsAsync(clinic);

            // Act
            var result = await service.UpdateClinicAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(clinic.LogoUrl);
            _fileStorageServiceMock.Verify(x => x.UploadImageAsync(It.IsAny<Microsoft.AspNetCore.Http.IFormFile>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _fileStorageServiceMock.Verify(x => x.DeleteImageAsync("old-image"), Times.Once);
        }

        [Fact]
        public async Task UpdateClinicAsync_WhenExceptionOccurs_ShouldDeleteNewImageAndThrow()
        {
            // Arrange
            var service = CreateService();
            var request = ClinicBuilder.CreateAndEditClinicDto(includeLogo: true);
            var clinic = ClinicBuilder.CreateClinicEntity(id: ClinicId, logoUrl: "old-image");

            _clinicRepositoryMock
                .Setup(x => x.GetClinicByIdAsync(ClinicId, true))
                .ReturnsAsync(clinic);

            _fileStorageServiceMock
                .Setup(x => x.UploadImageAsync(request.LogoUrl!))
                .ReturnsAsync(UploadedImageId);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new Exception("Database failure"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.UpdateClinicAsync(request));

            _fileStorageServiceMock.Verify(x => x.UploadImageAsync(request.LogoUrl!), Times.Once);
            _fileStorageServiceMock.Verify(x => x.DeleteImageAsync(UploadedImageId), Times.Once); // Clean up
        }

        #endregion

        #region GetClinicAsync

        [Fact]
        public async Task GetClinicAsync_WhenClinicNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();

            _clinicRepositoryMock
                .Setup(x => x.GetClinicByIdAsync(ClinicId, false))
                .ReturnsAsync((Clinic?)null);

            // Act
            var result = await service.GetClinicAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetClinicAsync_WhenClinicFoundAndLogoIsNull_ShouldReturnResponseWithoutGetFileUrl()
        {
            // Arrange
            var service = CreateService();
            var clinic = ClinicBuilder.CreateClinicEntity(id: ClinicId, logoUrl: null);

            _clinicRepositoryMock
                .Setup(x => x.GetClinicByIdAsync(ClinicId, false))
                .ReturnsAsync(clinic);

            // Act
            var result = await service.GetClinicAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(clinic.Name, result.Data.Name);
            _fileStorageServiceMock.Verify(x => x.GetFileUrl(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetClinicAsync_WhenClinicFoundAndLogoExists_ShouldReturnResponseWithGetFileUrl()
        {
            // Arrange
            var service = CreateService();
            var clinic = ClinicBuilder.CreateClinicEntity(id: ClinicId, logoUrl: "image.png");
            string expectedUrl = "https://storage.com/image.png";

            _clinicRepositoryMock
                .Setup(x => x.GetClinicByIdAsync(ClinicId, false))
                .ReturnsAsync(clinic);

            _fileStorageServiceMock
                .Setup(x => x.GetFileUrl("image.png"))
                .Returns(expectedUrl);

            // Act
            var result = await service.GetClinicAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(expectedUrl, result.Data.LogoUrl);
            _fileStorageServiceMock.Verify(x => x.GetFileUrl("image.png"), Times.Once);
        }

        #endregion
    }
}


