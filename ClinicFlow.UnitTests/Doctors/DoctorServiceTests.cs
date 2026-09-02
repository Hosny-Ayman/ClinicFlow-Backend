using AutoMapper;
using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Common.Security;
using ClinicFlow.Application.Features.Doctors;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Application.Features.Doctors.DTOs.Responses;
using ClinicFlow.Application.Features.DoctorSchedules;
using ClinicFlow.Application.Features.Users;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClinicFlow.UnitTests.Doctors
{
    public class DoctorServiceTests
    {
        private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ISpecialtyRepository> _specialtyRepositoryMock;
        private readonly Mock<ILogger<DoctorService>> _loggerMock;
        private readonly Mock<ICheckService> _authorizationServiceMock;
        private readonly Mock<IDoctorQueryService> _queryServiceMock;
        
        private readonly Mock<IUserQueryService> _userQueryServiceMock;
        private readonly Mock<IDoctorScheduleRepository> _doctorScheduleRepositoryMock;
        
        private readonly IMapper _mapper;

        private const int ClinicId = 10;
        private const int CurrentUserId = 99;

        public DoctorServiceTests()
        {
            _doctorRepositoryMock = DoctorMocks.DoctorRepository();
            _unitOfWorkMock = CommonMocks.UnitOfWork();
            _fileStorageServiceMock = DoctorMocks.FileStorageService();
            _userRepositoryMock = DoctorMocks.UserRepository();
            _userRoleRepositoryMock = DoctorMocks.UserRoleRepository();
            _currentUserServiceMock = CommonMocks.CurrentUserService();
            _specialtyRepositoryMock = DoctorMocks.SpecialtyRepository();
            _loggerMock = CommonMocks.Logger<DoctorService>();
            _authorizationServiceMock = DoctorMocks.CheckService();
            _queryServiceMock = DoctorMocks.QueryService();
            
            _userQueryServiceMock = DoctorMocks.UserQueryService();
            _doctorScheduleRepositoryMock = DoctorScheduleMocks.DoctorScheduleRepository();

            _currentUserServiceMock.Setup(x => x.ClinicId).Returns(ClinicId);
            _currentUserServiceMock.Setup(x => x.UserId).Returns(CurrentUserId);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(DoctorProfile).Assembly);
            });
            _mapper = config.CreateMapper();
        }

        private DoctorService CreateService()
        {
            var userService = new UserService(
                _userQueryServiceMock.Object,
                _currentUserServiceMock.Object,
                _userRepositoryMock.Object,
                _mapper,
                _userRoleRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _authorizationServiceMock.Object
            );

            var scheduleService = new DoctorScheduleService(
                _doctorScheduleRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _mapper,
                _currentUserServiceMock.Object,
                _doctorRepositoryMock.Object
            );

            return new DoctorService(
                _doctorRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _mapper,
                _fileStorageServiceMock.Object,
                _userRepositoryMock.Object,
                _userRoleRepositoryMock.Object,
                _currentUserServiceMock.Object,
                userService,
                _specialtyRepositoryMock.Object,
                _loggerMock.Object,
                _authorizationServiceMock.Object,
                _queryServiceMock.Object,
                scheduleService
            );
        }

        #region CreateDoctorStepsAsync Tests

        [Fact]
        public async Task CreateDoctorStepsAsync_WhenUserNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            var request = DoctorBuilder.CreateDoctorDto();

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(CurrentUserId, ClinicId, true))
                .ReturnsAsync((User?)null);

            // Act
            var result = await service.CreateDoctorStepsAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            _userRoleRepositoryMock.Verify(x => x.AssignRoleAsync(It.IsAny<User>(), It.IsAny<RoleEnum>()), Times.Never);
            _doctorRepositoryMock.Verify(x => x.AddDoctorAsync(It.IsAny<Doctor>()), Times.Never);
        }

        [Fact]
        public async Task CreateDoctorStepsAsync_WhenUserAlreadyHasRole_ShouldReturnConflict()
        {
            // Arrange
            var service = CreateService();
            var request = DoctorBuilder.CreateDoctorDto();
            var user = DoctorBuilder.CreateUserEntity();

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(CurrentUserId, ClinicId, true))
                .ReturnsAsync(user);

            _userRoleRepositoryMock
                .Setup(x => x.HasRoleAsync(CurrentUserId, RoleEnum.Doctor))
                .ReturnsAsync(true);

            // Act
            var result = await service.CreateDoctorStepsAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Conflict, result.Status);
            _userRoleRepositoryMock.Verify(x => x.AssignRoleAsync(It.IsAny<User>(), It.IsAny<RoleEnum>()), Times.Never);
            _doctorRepositoryMock.Verify(x => x.AddDoctorAsync(It.IsAny<Doctor>()), Times.Never);
        }

        [Fact]
        public async Task CreateDoctorStepsAsync_WhenSuccessWithoutImage_ShouldAddAndSave()
        {
            // Arrange
            var service = CreateService();
            var request = DoctorBuilder.CreateDoctorDto(profileImageFileName: null);
            
            var user = DoctorBuilder.CreateUserEntity();

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(CurrentUserId, ClinicId, true))
                .ReturnsAsync(user);

            _userRoleRepositoryMock
                .Setup(x => x.HasRoleAsync(CurrentUserId, RoleEnum.Doctor))
                .ReturnsAsync(false);

            _doctorRepositoryMock
                .Setup(x => x.AddDoctorAsync(It.IsAny<Doctor>()))
                .Callback<Doctor>(d => d.Id = 55)
                .ReturnsAsync(55);

            // Act
            var result = await service.CreateDoctorStepsAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(55, result.Data);
            _userRoleRepositoryMock.Verify(x => x.AssignRoleAsync(user, RoleEnum.Doctor), Times.Once);
            _fileStorageServiceMock.Verify(x => x.UploadImageAsync(It.IsAny<IFormFile>()), Times.Never);
            _doctorRepositoryMock.Verify(x => x.AddDoctorAsync(It.IsAny<Doctor>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateDoctorStepsAsync_WhenSuccessWithImage_ShouldUploadAddAndSave()
        {
            // Arrange
            var service = CreateService();
            var request = DoctorBuilder.CreateDoctorDto(profileImageFileName: "pic.jpg");
            var user = DoctorBuilder.CreateUserEntity();
            var imageId = "uploaded-image-id";

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(CurrentUserId, ClinicId, true))
                .ReturnsAsync(user);

            _userRoleRepositoryMock
                .Setup(x => x.HasRoleAsync(CurrentUserId, RoleEnum.Doctor))
                .ReturnsAsync(false);

            _fileStorageServiceMock
                .Setup(x => x.UploadImageAsync(request.ProfileImage!))
                .ReturnsAsync(imageId);

            _doctorRepositoryMock
                .Setup(x => x.AddDoctorAsync(It.IsAny<Doctor>()))
                .Callback<Doctor>(d => 
                {
                    Assert.Equal(imageId, d.ProfileImageUrl);
                    d.Id = 55;
                })
                .ReturnsAsync(55);

            // Act
            var result = await service.CreateDoctorStepsAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(55, result.Data);
            _fileStorageServiceMock.Verify(x => x.UploadImageAsync(request.ProfileImage!), Times.Once);
            _doctorRepositoryMock.Verify(x => x.AddDoctorAsync(It.IsAny<Doctor>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateDoctorStepsAsync_WhenExceptionAfterUpload_ShouldDeleteImageAndThrow()
        {
            // Arrange
            var service = CreateService();
            var request = DoctorBuilder.CreateDoctorDto(profileImageFileName: "pic.jpg");
            var user = DoctorBuilder.CreateUserEntity();
            var imageId = "uploaded-image-id";

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(CurrentUserId, ClinicId, true))
                .ReturnsAsync(user);

            _userRoleRepositoryMock
                .Setup(x => x.HasRoleAsync(CurrentUserId, RoleEnum.Doctor))
                .ReturnsAsync(false);

            _fileStorageServiceMock
                .Setup(x => x.UploadImageAsync(request.ProfileImage!))
                .ReturnsAsync(imageId);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new Exception("DB Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.CreateDoctorStepsAsync(request));

            _fileStorageServiceMock.Verify(x => x.DeleteImageAsync(imageId), Times.Once);
        }

        #endregion

        #region CreateDoctorAsync Tests

        [Fact]
        public async Task CreateDoctorAsync_WhenUserCreationReturnsNull_ShouldReturnConflict()
        {
            // Arrange
            var service = CreateService();
            var doctorDto = DoctorBuilder.CreateDoctorDto();
            var userDto = DoctorBuilder.CreateUserDto();

            _userRepositoryMock
                .Setup(x => x.IsEmailExitsAsync(userDto.Email))
                .ReturnsAsync(true);

            // Act
            var result = await service.CreateDoctorAsync(doctorDto, userDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Conflict, result.Status);
            _userRoleRepositoryMock.Verify(x => x.AssignRoleAsync(It.IsAny<User>(), It.IsAny<RoleEnum>()), Times.Never);
            _doctorRepositoryMock.Verify(x => x.AddDoctorAsync(It.IsAny<Doctor>()), Times.Never);
        }

        [Fact]
        public async Task CreateDoctorAsync_WhenUserAlreadyHasRole_ShouldReturnConflict()
        {
            // Arrange
            var service = CreateService();
            var doctorDto = DoctorBuilder.CreateDoctorDto();
            var userDto = DoctorBuilder.CreateUserDto();

            _userRepositoryMock.Setup(x => x.IsEmailExitsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRepositoryMock.Setup(x => x.IsPhoneExitsAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            _userRoleRepositoryMock
                .Setup(x => x.HasRoleAsync(It.IsAny<int>(), RoleEnum.Doctor))
                .ReturnsAsync(true);

            // Act
            var result = await service.CreateDoctorAsync(doctorDto, userDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Conflict, result.Status);
            _doctorRepositoryMock.Verify(x => x.AddDoctorAsync(It.IsAny<Doctor>()), Times.Never);
        }

        [Fact]
        public async Task CreateDoctorAsync_WhenSuccessWithoutImage_ShouldAddAndSave()
        {
            // Arrange
            var service = CreateService();
            var doctorDto = DoctorBuilder.CreateDoctorDto(profileImageFileName: null);
            
            var userDto = DoctorBuilder.CreateUserDto();

            _userRepositoryMock.Setup(x => x.IsEmailExitsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRepositoryMock.Setup(x => x.IsPhoneExitsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRoleRepositoryMock.Setup(x => x.HasRoleAsync(It.IsAny<int>(), RoleEnum.Doctor)).ReturnsAsync(false);

            _doctorRepositoryMock
                .Setup(x => x.AddDoctorAsync(It.IsAny<Doctor>()))
                .Callback<Doctor>(d => d.Id = 100)
                .ReturnsAsync(100);

            // Act
            var result = await service.CreateDoctorAsync(doctorDto, userDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(100, result.Data);
            _userRoleRepositoryMock.Verify(x => x.AssignRoleAsync(It.IsAny<User>(), RoleEnum.Doctor), Times.Once);
            _fileStorageServiceMock.Verify(x => x.UploadImageAsync(It.IsAny<IFormFile>()), Times.Never);
            _doctorRepositoryMock.Verify(x => x.AddDoctorAsync(It.IsAny<Doctor>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateDoctorAsync_WhenSuccessWithImage_ShouldUploadAddAndSave()
        {
            // Arrange
            var service = CreateService();
            var doctorDto = DoctorBuilder.CreateDoctorDto(profileImageFileName: "pic.jpg");
            var userDto = DoctorBuilder.CreateUserDto();
            var imageId = "new-image-id";

            _userRepositoryMock.Setup(x => x.IsEmailExitsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRepositoryMock.Setup(x => x.IsPhoneExitsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRoleRepositoryMock.Setup(x => x.HasRoleAsync(It.IsAny<int>(), RoleEnum.Doctor)).ReturnsAsync(false);

            _fileStorageServiceMock
                .Setup(x => x.UploadImageAsync(doctorDto.ProfileImage!))
                .ReturnsAsync(imageId);

            _doctorRepositoryMock
                .Setup(x => x.AddDoctorAsync(It.IsAny<Doctor>()))
                .Callback<Doctor>(d => d.Id = 100)
                .ReturnsAsync(100);

            // Act
            var result = await service.CreateDoctorAsync(doctorDto, userDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(100, result.Data);
            _fileStorageServiceMock.Verify(x => x.UploadImageAsync(doctorDto.ProfileImage!), Times.Once);
            _doctorRepositoryMock.Verify(x => x.AddDoctorAsync(It.Is<Doctor>(d => d.ProfileImageUrl == imageId)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateDoctorAsync_WhenExceptionAfterUpload_ShouldDeleteImageAndThrow()
        {
            // Arrange
            var service = CreateService();
            var doctorDto = DoctorBuilder.CreateDoctorDto(profileImageFileName: "pic.jpg");
            var userDto = DoctorBuilder.CreateUserDto();
            var imageId = "new-image-id";

            _userRepositoryMock.Setup(x => x.IsEmailExitsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRepositoryMock.Setup(x => x.IsPhoneExitsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRoleRepositoryMock.Setup(x => x.HasRoleAsync(It.IsAny<int>(), RoleEnum.Doctor)).ReturnsAsync(false);

            _fileStorageServiceMock
                .Setup(x => x.UploadImageAsync(doctorDto.ProfileImage!))
                .ReturnsAsync(imageId);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new Exception("DB Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.CreateDoctorAsync(doctorDto, userDto));
            _fileStorageServiceMock.Verify(x => x.DeleteImageAsync(imageId), Times.Once);
        }

        #endregion

        #region GetDoctorFullInforamtionByIdAsync Tests

        [Fact]
        public async Task GetDoctorFullInforamtionByIdAsync_WhenUserNull_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            int doctorId = 5;

            _userRepositoryMock
                .Setup(x => x.GetUserByDoctorIdAsync(doctorId, ClinicId, false))
                .ReturnsAsync((User?)null);

            // Act
            var result = await service.GetDoctorFullInforamtionByIdAsync(doctorId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            _doctorRepositoryMock.Verify(x => x.GetDoctorByIdAsync(It.IsAny<int>(), It.IsAny<int>(), false), Times.Never);
        }

        [Fact]
        public async Task GetDoctorFullInforamtionByIdAsync_WhenDoctorNull_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            int doctorId = 5;
            var user = DoctorBuilder.CreateUserEntity();

            _userRepositoryMock
                .Setup(x => x.GetUserByDoctorIdAsync(doctorId, ClinicId, false))
                .ReturnsAsync(user);

            _doctorRepositoryMock
                .Setup(x => x.GetDoctorByIdAsync(doctorId, ClinicId, false))
                .ReturnsAsync((Doctor?)null);

            _authorizationServiceMock
                .Setup(x => x.EnsureCanManageUser(user.Id))
                .Returns(true);

            // Act
            var result = await service.GetDoctorFullInforamtionByIdAsync(doctorId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetDoctorFullInforamtionByIdAsync_WhenAuthFails_ShouldReturnForbidden()
        {
            // Arrange
            var service = CreateService();
            int doctorId = 5;
            var user = DoctorBuilder.CreateUserEntity(id: 10);
            var doctor = DoctorBuilder.CreateDoctorEntity(id: doctorId, userId: 10);

            _userRepositoryMock.Setup(x => x.GetUserByDoctorIdAsync(doctorId, ClinicId, false)).ReturnsAsync(user);
            _doctorRepositoryMock.Setup(x => x.GetDoctorByIdAsync(doctorId, ClinicId, false)).ReturnsAsync(doctor);

            _authorizationServiceMock
                .Setup(x => x.EnsureCanManageUser(10))
                .Returns(false);

            // Act
            var result = await service.GetDoctorFullInforamtionByIdAsync(doctorId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Forbidden, result.Status);
        }

        [Fact]
        public async Task GetDoctorFullInforamtionByIdAsync_WhenSuccess_ShouldReturnData()
        {
            // Arrange
            var service = CreateService();
            int doctorId = 5;
            var user = DoctorBuilder.CreateUserEntity(id: 10);
            var doctor = DoctorBuilder.CreateDoctorEntity(id: doctorId, userId: 10);
            doctor.ProfileImageUrl = "image-id";

            _userRepositoryMock.Setup(x => x.GetUserByDoctorIdAsync(doctorId, ClinicId, false)).ReturnsAsync(user);
            _doctorRepositoryMock.Setup(x => x.GetDoctorByIdAsync(doctorId, ClinicId, false)).ReturnsAsync(doctor);
            _authorizationServiceMock.Setup(x => x.EnsureCanManageUser(10)).Returns(true);
            _fileStorageServiceMock.Setup(x => x.GetFileUrl("image-id")).Returns("http://image-url");

            // Act
            var result = await service.GetDoctorFullInforamtionByIdAsync(doctorId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("http://image-url", result.Data.Doctor.ProfileImageUrl);
            Assert.Equal("Cardiology", result.Data.Doctor.SpecialtieName);
        }

        #endregion

        #region GetDoctorFullInforamtionByUserIdAsync Tests

        [Fact]
        public async Task GetDoctorFullInforamtionByUserIdAsync_WhenDoctorNull_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            int targetUserId = 10;

            _doctorRepositoryMock
                .Setup(x => x.GetDoctorByUserIdAsync(targetUserId, ClinicId, false))
                .ReturnsAsync((Doctor?)null);

            // Act
            var result = await service.GetDoctorFullInforamtionByUserIdAsync(targetUserId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetDoctorFullInforamtionByUserIdAsync_WhenAuthFails_ShouldReturnForbidden()
        {
            // Arrange
            var service = CreateService();
            int targetUserId = 10;
            var doctor = DoctorBuilder.CreateDoctorEntity(userId: targetUserId);

            _doctorRepositoryMock.Setup(x => x.GetDoctorByUserIdAsync(targetUserId, ClinicId, false)).ReturnsAsync(doctor);
            _authorizationServiceMock.Setup(x => x.EnsureCanManageUser(targetUserId)).Returns(false);

            // Act
            var result = await service.GetDoctorFullInforamtionByUserIdAsync(targetUserId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Forbidden, result.Status);
        }

        [Fact]
        public async Task GetDoctorFullInforamtionByUserIdAsync_WhenSuccess_ShouldReturnData()
        {
            // Arrange
            var service = CreateService();
            int targetUserId = 10;
            var doctor = DoctorBuilder.CreateDoctorEntity(userId: targetUserId);
            var user = DoctorBuilder.CreateUserEntity(id: targetUserId);
            doctor.User = user;

            _doctorRepositoryMock.Setup(x => x.GetDoctorByUserIdAsync(targetUserId, ClinicId, false)).ReturnsAsync(doctor);
            _authorizationServiceMock.Setup(x => x.EnsureCanManageUser(targetUserId)).Returns(true);

            // Act
            var result = await service.GetDoctorFullInforamtionByUserIdAsync(targetUserId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Cardiology", result.Data.SpecialtieName);
        }

        #endregion

        #region UpdateDoctorAsync Tests

        [Fact]
        public async Task UpdateDoctorAsync_WhenAuthFails_ShouldReturnForbidden()
        {
            // Arrange
            var service = CreateService();
            var userDto = DoctorBuilder.UpdateUserDto(id: 10);
            var doctorDto = DoctorBuilder.UpdateDoctorDto();

            _authorizationServiceMock.Setup(x => x.EnsureCanManageUser(10)).Returns(false);

            // Act
            var result = await service.UpdateDoctorAsync(userDto, doctorDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Forbidden, result.Status);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateDoctorAsync_WhenUserNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            var userDto = DoctorBuilder.UpdateUserDto(id: 10);
            var doctorDto = DoctorBuilder.UpdateDoctorDto();

            _authorizationServiceMock.Setup(x => x.EnsureCanManageUser(10)).Returns(true);
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(10, ClinicId, true)).ReturnsAsync((User?)null);

            // Act
            var result = await service.UpdateDoctorAsync(userDto, doctorDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateDoctorAsync_WhenDoctorNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            var userDto = DoctorBuilder.UpdateUserDto(id: 10);
            var doctorDto = DoctorBuilder.UpdateDoctorDto(id: 20);
            var user = DoctorBuilder.CreateUserEntity();

            _authorizationServiceMock.Setup(x => x.EnsureCanManageUser(10)).Returns(true);
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(10, ClinicId, true)).ReturnsAsync(user);
            _doctorRepositoryMock.Setup(x => x.GetDoctorByIdAsync(20, ClinicId, true)).ReturnsAsync((Doctor?)null);

            // Act
            var result = await service.UpdateDoctorAsync(userDto, doctorDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateDoctorAsync_WhenSuccessWithoutImageChange_ShouldSave()
        {
            // Arrange
            var service = CreateService();
            var userDto = DoctorBuilder.UpdateUserDto(id: 10);
            var doctorDto = DoctorBuilder.UpdateDoctorDto(id: 20);
            
            var user = DoctorBuilder.CreateUserEntity();
            var doctor = DoctorBuilder.CreateDoctorEntity(id: 20, profileImageUrl: "old-image");

            _authorizationServiceMock.Setup(x => x.EnsureCanManageUser(10)).Returns(true);
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(10, ClinicId, true)).ReturnsAsync(user);
            _doctorRepositoryMock.Setup(x => x.GetDoctorByIdAsync(20, ClinicId, true)).ReturnsAsync(doctor);

            // Act
            var result = await service.UpdateDoctorAsync(userDto, doctorDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("old-image", doctor.ProfileImageUrl);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _fileStorageServiceMock.Verify(x => x.UploadImageAsync(It.IsAny<IFormFile>()), Times.Never);
            _fileStorageServiceMock.Verify(x => x.DeleteImageAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateDoctorAsync_WhenSuccessWithNewImage_ShouldUploadAndSave()
        {
            // Arrange
            var service = CreateService();
            var userDto = DoctorBuilder.UpdateUserDto(id: 10);
            var fileMock = new Mock<IFormFile>();
            var doctorDto = DoctorBuilder.UpdateDoctorDto(id: 20, newProfileImageUrl: fileMock.Object);
            
            var user = DoctorBuilder.CreateUserEntity();
            var doctor = DoctorBuilder.CreateDoctorEntity(id: 20, profileImageUrl: "old-image");

            _authorizationServiceMock.Setup(x => x.EnsureCanManageUser(10)).Returns(true);
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(10, ClinicId, true)).ReturnsAsync(user);
            _doctorRepositoryMock.Setup(x => x.GetDoctorByIdAsync(20, ClinicId, true)).ReturnsAsync(doctor);

            _fileStorageServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync("new-image-id");

            // Act
            var result = await service.UpdateDoctorAsync(userDto, doctorDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("new-image-id", doctor.ProfileImageUrl);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _fileStorageServiceMock.Verify(x => x.UploadImageAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public async Task UpdateDoctorAsync_WhenSuccessWithImageDeletion_ShouldDeleteAndSave()
        {
            // Arrange
            var service = CreateService();
            var userDto = DoctorBuilder.UpdateUserDto(id: 10);
            var doctorDto = DoctorBuilder.UpdateDoctorDto(id: 20, isImageDeleted: true);
            
            var user = DoctorBuilder.CreateUserEntity();
            var doctor = DoctorBuilder.CreateDoctorEntity(id: 20, profileImageUrl: "old-image");

            _authorizationServiceMock.Setup(x => x.EnsureCanManageUser(10)).Returns(true);
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(10, ClinicId, true)).ReturnsAsync(user);
            _doctorRepositoryMock.Setup(x => x.GetDoctorByIdAsync(20, ClinicId, true)).ReturnsAsync(doctor);

            // Act
            var result = await service.UpdateDoctorAsync(userDto, doctorDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(doctor.ProfileImageUrl);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _fileStorageServiceMock.Verify(x => x.DeleteImageAsync("old-image"), Times.Once);
        }

        [Fact]
        public async Task UpdateDoctorAsync_WhenExceptionAfterNewImageUpload_ShouldDeleteNewImageAndThrow()
        {
            // Arrange
            var service = CreateService();
            var userDto = DoctorBuilder.UpdateUserDto(id: 10);
            var fileMock = new Mock<IFormFile>();
            var doctorDto = DoctorBuilder.UpdateDoctorDto(id: 20, newProfileImageUrl: fileMock.Object);
            
            var user = DoctorBuilder.CreateUserEntity();
            var doctor = DoctorBuilder.CreateDoctorEntity(id: 20, profileImageUrl: "old-image");

            _authorizationServiceMock.Setup(x => x.EnsureCanManageUser(10)).Returns(true);
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(10, ClinicId, true)).ReturnsAsync(user);
            _doctorRepositoryMock.Setup(x => x.GetDoctorByIdAsync(20, ClinicId, true)).ReturnsAsync(doctor);

            _fileStorageServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync("new-image-id");

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ThrowsAsync(new Exception("DB Error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.UpdateDoctorAsync(userDto, doctorDto));
            _fileStorageServiceMock.Verify(x => x.DeleteImageAsync("new-image-id"), Times.Once);
        }

        #endregion

        #region Read Methods Tests

        [Fact]
        public async Task GetAllDoctorsInformationsAsync_ShouldCallQueryServiceAndReturnWrappedData()
        {
            // Arrange
            var service = CreateService();
            var request = DoctorBuilder.CreateSearchRequest();
            var expectedResponse = new PagedResponse<GetAllDoctorsInformationsDtoResponse>(
                new List<GetAllDoctorsInformationsDtoResponse>(), 1, 10, 0
            );

            _queryServiceMock
                .Setup(x => x.GetAllDoctorsInformationsAsync(request, ClinicId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await service.GetAllDoctorsInformationsAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Same(expectedResponse, result.Data);
            _queryServiceMock.Verify(x => x.GetAllDoctorsInformationsAsync(request, ClinicId), Times.Once);
        }

        [Fact]
        public async Task GetAllDoctorsBySpecialtyAsync_ShouldCallQueryServiceAndTransformImages()
        {
            // Arrange
            var service = CreateService();
            int specialtyId = 3;
            var responseList = new List<GetAllDoctorsInformationsDtoResponse>
            {
                new GetAllDoctorsInformationsDtoResponse { Image = "img1" },
                new GetAllDoctorsInformationsDtoResponse { Image = null }
            };

            _queryServiceMock
                .Setup(x => x.GetAllDoctorsBySpecialtyAsync(specialtyId, ClinicId))
                .ReturnsAsync(responseList);

            _fileStorageServiceMock
                .Setup(x => x.GetFileUrl("img1"))
                .Returns("http://url/img1");

            // Act
            var result = await service.GetAllDoctorsBySpecialtyAsync(specialtyId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data!.Count);
            Assert.Equal("http://url/img1", result.Data[0].Image);
            Assert.Null(result.Data[1].Image);
            _queryServiceMock.Verify(x => x.GetAllDoctorsBySpecialtyAsync(specialtyId, ClinicId), Times.Once);
        }

        #endregion
    }
}
