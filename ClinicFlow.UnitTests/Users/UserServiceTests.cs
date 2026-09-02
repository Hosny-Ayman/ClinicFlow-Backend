using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Features.Authentication.DTOs.Responses;
using ClinicFlow.Application.Common.Security;
using ClinicFlow.Application.Features.Users;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Application.Features.Users.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Moq;

namespace ClinicFlow.UnitTests.Users
{
    public class UserServiceTests
    {
        private readonly Mock<IUserQueryService> _userQueryServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IMapper _mapper;
        private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICheckService> _authorizationServiceMock;

        private const int ClinicId = 10;
        private const string CurrentUserEmail = "admin@example.com";

        public UserServiceTests()
        {
            _userQueryServiceMock = UserQueryMocks.UserQueryService();
            _currentUserServiceMock = CommonMocks.CurrentUserService();
            _userRepositoryMock = AuthenticationMocks.UserRepository();
            _userRoleRepositoryMock = DoctorMocks.UserRoleRepository();
            _unitOfWorkMock = CommonMocks.UnitOfWork();
            _authorizationServiceMock = DoctorMocks.CheckService();

            _currentUserServiceMock.Setup(x => x.ClinicId).Returns(ClinicId);
            _currentUserServiceMock.Setup(x => x.Email).Returns(CurrentUserEmail);
            _currentUserServiceMock.Setup(x => x.IsAuthenticated).Returns(true);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(UserProfile).Assembly);
            });
            _mapper = config.CreateMapper();
        }

        private UserService CreateService()
        {
            return new UserService(
                _userQueryServiceMock.Object,
                _currentUserServiceMock.Object,
                _userRepositoryMock.Object,
                _mapper,
                _userRoleRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _authorizationServiceMock.Object
            );
        }

        #region GetCurrentUserAsync

        [Fact]
        public async Task GetCurrentUserAsync_WhenNotAuthenticated_ShouldReturnUnauthorized()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.IsAuthenticated).Returns(false);
            var service = CreateService();

            // Act
            var result = await service.GetCurrentUserAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Unauthorized, result.Status);
        }

        [Fact]
        public async Task GetCurrentUserAsync_WhenUserNull_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            _userQueryServiceMock
                .Setup(x => x.GetUserProfilByEmaileAsync(CurrentUserEmail))
                .ReturnsAsync((CurrentUserDto?)null);

            // Act
            var result = await service.GetCurrentUserAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetCurrentUserAsync_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var service = CreateService();
            var currentUserDto = new CurrentUserDto { Id = 1, Email = CurrentUserEmail };

            _userQueryServiceMock
                .Setup(x => x.GetUserProfilByEmaileAsync(CurrentUserEmail))
                .ReturnsAsync(currentUserDto);

            // Act
            var result = await service.GetCurrentUserAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Same(currentUserDto, result.Data);
        }

        #endregion

        #region CreateReceptionistAsync

        [Fact]
        public async Task CreateReceptionistAsync_WhenUserAlreadyExists_ShouldReturnConflict()
        {
            // Arrange
            var service = CreateService();
            var request = UserBuilder.CreateAndEditUserDto();

            _userRepositoryMock
                .Setup(x => x.IsEmailExitsAsync(request.Email))
                .ReturnsAsync(true); // Simulating existing email

            // Act
            var result = await service.CreateReceptionistAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Conflict, result.Status);
        }

        [Fact]
        public async Task CreateReceptionistAsync_WhenValidRequest_ShouldCreateUserAndAssignRole()
        {
            // Arrange
            var service = CreateService();
            var request = UserBuilder.CreateAndEditUserDto();

            _userRepositoryMock
                .Setup(x => x.IsEmailExitsAsync(request.Email))
                .ReturnsAsync(false);
            _userRepositoryMock
                .Setup(x => x.IsPhoneExitsAsync(request.PhoneNumber))
                .ReturnsAsync(false);

            _userRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => u.Id = 99)
                .ReturnsAsync(99);

            // Act
            var result = await service.CreateReceptionistAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(99, result.Data);

            _userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u => 
                u.Person.FirstName == request.FirstName &&
                u.ClinicId == ClinicId &&
                u.IsActive == true
            )), Times.Once);

            _userRoleRepositoryMock.Verify(x => x.AssignRoleAsync(It.IsAny<User>(), RoleEnum.Receptionist), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region GetUserInformationByIdAsync

        [Fact]
        public async Task GetUserInformationByIdAsync_WhenCannotManageUser_ShouldReturnForbidden()
        {
            // Arrange
            var service = CreateService();
            int userId = 5;

            _authorizationServiceMock
                .Setup(x => x.EnsureCanManageUser(userId))
                .Returns(false);

            // Act
            var result = await service.GetUserInformationByIdAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Forbidden, result.Status);
        }

        [Fact]
        public async Task GetUserInformationByIdAsync_WhenUserNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            int userId = 5;

            _authorizationServiceMock
                .Setup(x => x.EnsureCanManageUser(userId))
                .Returns(true);

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(userId, ClinicId, false))
                .ReturnsAsync((User?)null);

            // Act
            var result = await service.GetUserInformationByIdAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetUserInformationByIdAsync_WhenUserExists_ShouldReturnMappedUser()
        {
            // Arrange
            var service = CreateService();
            int userId = 5;
            var user = UserBuilder.CreateUserEntity(id: userId, clinicId: ClinicId);

            _authorizationServiceMock
                .Setup(x => x.EnsureCanManageUser(userId))
                .Returns(true);

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(userId, ClinicId, false))
                .ReturnsAsync(user);

            // Act
            var result = await service.GetUserInformationByIdAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(user.Person.FirstName, result.Data.FirstName);
        }

        #endregion

        #region UpdateUserAsync

        [Fact]
        public async Task UpdateUserAsync_WhenCannotManageUser_ShouldReturnForbidden()
        {
            // Arrange
            var service = CreateService();
            var request = UserBuilder.UpdateUserDto(id: 5);

            _authorizationServiceMock
                .Setup(x => x.EnsureCanManageUser(request.Id))
                .Returns(false);

            // Act
            var result = await service.UpdateUserAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Forbidden, result.Status);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenUserNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            var request = UserBuilder.UpdateUserDto(id: 5);

            _authorizationServiceMock
                .Setup(x => x.EnsureCanManageUser(request.Id))
                .Returns(true);

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(request.Id, ClinicId, true))
                .ReturnsAsync((User?)null);

            // Act
            var result = await service.UpdateUserAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenUserExists_ShouldUpdateAndSave()
        {
            // Arrange
            var service = CreateService();
            var request = UserBuilder.UpdateUserDto(id: 5, password: "");
            var user = UserBuilder.CreateUserEntity(id: 5, clinicId: ClinicId);
            string oldPasswordHash = user.PasswordHash;

            _authorizationServiceMock
                .Setup(x => x.EnsureCanManageUser(request.Id))
                .Returns(true);

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(request.Id, ClinicId, true))
                .ReturnsAsync(user);

            // Act
            var result = await service.UpdateUserAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(request.FirstName, user.Person.FirstName);
            Assert.Equal(oldPasswordHash, user.PasswordHash); // Password wasn't updated since it was empty
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenPasswordProvided_ShouldHashNewPassword()
        {
            // Arrange
            var service = CreateService();
            var request = UserBuilder.UpdateUserDto(id: 5, password: "NewStrongPassword123!");
            var user = UserBuilder.CreateUserEntity(id: 5, clinicId: ClinicId);
            string oldPasswordHash = user.PasswordHash;

            _authorizationServiceMock
                .Setup(x => x.EnsureCanManageUser(request.Id))
                .Returns(true);

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(request.Id, ClinicId, true))
                .ReturnsAsync(user);

            // Act
            var result = await service.UpdateUserAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEqual(oldPasswordHash, user.PasswordHash); // Password was updated
            Assert.True(BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash));
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region ToggleUserStatusAsync

        [Fact]
        public async Task ToggleUserStatusAsync_WhenUserDoesNotExist_ShouldReturnBadRequest()
        {
            // Arrange
            var service = CreateService();
            int userId = 5;

            _userRepositoryMock
                .Setup(x => x.IsUserExistsByIdAsync(userId, ClinicId))
                .ReturnsAsync(false);

            // Act
            var result = await service.ToggleUserStatusAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);
        }

        [Fact]
        public async Task ToggleUserStatusAsync_WhenToggleFails_ShouldReturnFailure()
        {
            // Arrange
            var service = CreateService();
            int userId = 5;

            _userRepositoryMock
                .Setup(x => x.IsUserExistsByIdAsync(userId, ClinicId))
                .ReturnsAsync(true);

            _userRepositoryMock
                .Setup(x => x.ToggleUserStatusAsync(userId, ClinicId))
                .ReturnsAsync(false);

            // Act
            var result = await service.ToggleUserStatusAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.ServerError, result.Status);
        }

        [Fact]
        public async Task ToggleUserStatusAsync_WhenToggleSucceeds_ShouldReturnSuccess()
        {
            // Arrange
            var service = CreateService();
            int userId = 5;

            _userRepositoryMock
                .Setup(x => x.IsUserExistsByIdAsync(userId, ClinicId))
                .ReturnsAsync(true);

            _userRepositoryMock
                .Setup(x => x.ToggleUserStatusAsync(userId, ClinicId))
                .ReturnsAsync(true);

            // Act
            var result = await service.ToggleUserStatusAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        #endregion

        #region GetAllReceptionistsformationsAsync

        [Fact]
        public async Task GetAllReceptionistsformationsAsync_ShouldCallQueryService()
        {
            // Arrange
            var service = CreateService();
            var request = UserBuilder.CreateReceptionistsSearchDtoRequest();
            var expectedResponse = new PagedResponse<GetAllReceptionistsDtoRequest>(
                new List<GetAllReceptionistsDtoRequest>(), 1, 10, 0
            );

            _userQueryServiceMock
                .Setup(x => x.GetAllReceptionistsformationsAsync(request, ClinicId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await service.GetAllReceptionistsformationsAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Same(expectedResponse, result.Data);
            _userQueryServiceMock.Verify(x => x.GetAllReceptionistsformationsAsync(request, ClinicId), Times.Once);
        }

        #endregion

        #region AddUserInsideProjectOnlyAsync

        [Fact]
        public async Task AddUserInsideProjectOnlyAsync_WhenEmailExists_ShouldReturnNull()
        {
            // Arrange
            var service = CreateService();
            var request = UserBuilder.CreateAndEditUserDto();

            _userRepositoryMock
                .Setup(x => x.IsEmailExitsAsync(request.Email))
                .ReturnsAsync(true);

            // Act
            var result = await service.AddUserInsideProjectOnlyWithClinicAlreadyExistsAsync(request);

            // Assert
            Assert.Null(result);
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AddUserInsideProjectOnlyAsync_WhenPhoneExists_ShouldReturnNull()
        {
            // Arrange
            var service = CreateService();
            var request = UserBuilder.CreateAndEditUserDto();

            _userRepositoryMock
                .Setup(x => x.IsEmailExitsAsync(request.Email))
                .ReturnsAsync(false);
            _userRepositoryMock
                .Setup(x => x.IsPhoneExitsAsync(request.PhoneNumber))
                .ReturnsAsync(true);

            // Act
            var result = await service.AddUserInsideProjectOnlyWithClinicAlreadyExistsAsync(request);

            // Assert
            Assert.Null(result);
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AddUserInsideProjectOnlyAsync_WhenValid_ShouldReturnUserAndCallAddAsync()
        {
            // Arrange
            var service = CreateService();
            var request = UserBuilder.CreateAndEditUserDto();

            _userRepositoryMock
                .Setup(x => x.IsEmailExitsAsync(request.Email))
                .ReturnsAsync(false);
            _userRepositoryMock
                .Setup(x => x.IsPhoneExitsAsync(request.PhoneNumber))
                .ReturnsAsync(false);

            _userRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => u.Id = 99)
                .ReturnsAsync(99);

            // Act
            var result = await service.AddUserInsideProjectOnlyWithClinicAlreadyExistsAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(99, result.Id);
            Assert.Equal(request.FirstName, result.Person.FirstName);
            Assert.Equal(request.Email, result.Person.Email);
            Assert.Equal(request.PhoneNumber, result.Person.PhoneNumber);
            Assert.True(BCrypt.Net.BCrypt.Verify(request.Password, result.PasswordHash));
            Assert.Equal(ClinicId, result.ClinicId);

            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        }

        #endregion

        #region UpdateUserInsideProjectOnlyAsync

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateUserInsideProjectOnlyAsync_WhenPasswordIsNullOrWhiteSpace_ShouldNotUpdatePassword(string? emptyPassword)
        {
            // Arrange
            var service = CreateService();
            var request = UserBuilder.UpdateUserDto(id: 5, password: emptyPassword!);
            var user = UserBuilder.CreateUserEntity(id: 5, clinicId: ClinicId);
            string oldPasswordHash = user.PasswordHash;

            // Act
            service.UpdateUserInsideProjectOnlyAsync(user, request);

            // Assert
            Assert.Equal(request.FirstName, user.Person.FirstName);
            Assert.Equal(request.LastName, user.Person.LastName);
            Assert.Equal(request.Email, user.Person.Email);
            Assert.Equal(request.PhoneNumber, user.Person.PhoneNumber);
            Assert.Equal(request.IsActive, user.IsActive);
            Assert.Equal(oldPasswordHash, user.PasswordHash);
        }

        [Fact]
        public void UpdateUserInsideProjectOnlyAsync_WhenPasswordProvided_ShouldHashNewPassword()
        {
            // Arrange
            var service = CreateService();
            var request = UserBuilder.UpdateUserDto(id: 5, password: "NewStrongPassword123!");
            var user = UserBuilder.CreateUserEntity(id: 5, clinicId: ClinicId);
            string oldPasswordHash = user.PasswordHash;

            // Act
            service.UpdateUserInsideProjectOnlyAsync(user, request);

            // Assert
            Assert.Equal(request.FirstName, user.Person.FirstName);
            Assert.NotEqual(oldPasswordHash, user.PasswordHash);
            Assert.True(BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash));
        }

        #endregion

    }
}
