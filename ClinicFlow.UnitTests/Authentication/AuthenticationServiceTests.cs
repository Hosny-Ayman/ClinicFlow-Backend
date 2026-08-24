using ClinicFlow.Application.Features.Authentication;
using ClinicFlow.Domain.Entities;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Moq;

namespace ClinicFlow.UnitTests.Authentication
{
    public class AuthenticationServiceTests
    {

        [Fact]
        public async Task LoginAsync_WhenUserIsValid_ShouldLoginIn()
        {
            //Arrange
            var loginUserData = AuthenticationBuilder.Create(); 

            var userRepository = AuthenticationMocks.UserRepository();
            var mapper = CommonMocks.Mapper();
            var logger = CommonMocks.Logger<AuthenticationService>();
            var refreshTokenRepository = AuthenticationMocks.RefreshTokenRepository();
            var jwtProvider = AuthenticationMocks.JwtProvider();
            var refreshTokenGenerator = AuthenticationMocks.RefreshTokenGenerator();
            var refreshTokenHasher = AuthenticationMocks.RefreshTokenHasher();
            var jwtSettings = AuthenticationMocks.JwtSettings();
            var unitOfWork = CommonMocks.UnitOfWork();
            var currentUserService = CommonMocks.CurrentUserService();
            var cookieService = AuthenticationMocks.CookieService();

            var user = CommonBuilder.CreateUser();

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginUserData.Password);

            var accessToken = CommonBuilder.CreateAccessToken();
            var refreshToken = CommonBuilder.CreateRefreshToken();
            var hash = CommonBuilder.CreateHash();

            userRepository.Setup(repo => repo.GetUserByEmailAsync(loginUserData.Email, false)).ReturnsAsync(user);

            jwtProvider.Setup(repo => repo.GenerateToken(user)).Returns(accessToken);
            refreshTokenGenerator.Setup(repo => repo.Generate()).Returns(refreshToken);
            refreshTokenHasher.Setup(repo => repo.Hash(refreshToken)).Returns(hash);

            refreshTokenRepository.Setup(repo => repo.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
            unitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1); 

            var authenticationService = new AuthenticationService(userRepository.Object, mapper, logger.Object, refreshTokenRepository.Object, jwtProvider.Object,
            refreshTokenGenerator.Object, refreshTokenHasher.Object, jwtSettings, unitOfWork.Object, currentUserService.Object
            , cookieService.Object);



            //Act
            var resul = await authenticationService.LoginAsync(loginUserData);

            //Assert

            Assert.True(resul.IsSuccess);
            Assert.NotNull(resul.Data);

            Assert.Equal(accessToken, resul.Data.AccessToken);
            Assert.Equal(refreshToken, resul.Data.RefreshToken);

            refreshTokenRepository.Verify(repo => repo.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
            unitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_WhenUserIsNULL_ShouldNotLoginIn()
        {
            //Arrange
            var loginUserData = AuthenticationBuilder.Create();

            var userRepository = AuthenticationMocks.UserRepository();
            var mapper = CommonMocks.Mapper();
            var logger = CommonMocks.Logger<AuthenticationService>();
            var refreshTokenRepository = AuthenticationMocks.RefreshTokenRepository();
            var jwtProvider = AuthenticationMocks.JwtProvider();
            var refreshTokenGenerator = AuthenticationMocks.RefreshTokenGenerator();
            var refreshTokenHasher = AuthenticationMocks.RefreshTokenHasher();
            var jwtSettings = AuthenticationMocks.JwtSettings();
            var unitOfWork = CommonMocks.UnitOfWork();
            var currentUserService = CommonMocks.CurrentUserService();
            var cookieService = AuthenticationMocks.CookieService();

            User user = null;

            userRepository.Setup(repo => repo.GetUserByEmailAsync(loginUserData.Email, false)).ReturnsAsync(user);

            var authenticationService = new AuthenticationService(userRepository.Object, mapper, logger.Object, refreshTokenRepository.Object, jwtProvider.Object,
            refreshTokenGenerator.Object, refreshTokenHasher.Object, jwtSettings, unitOfWork.Object, currentUserService.Object
            , cookieService.Object);



            //Act
            var resul = await authenticationService.LoginAsync(loginUserData);

            //Assert

            Assert.False(resul.IsSuccess);
            Assert.Null(resul.Data);

        }

        [Fact]
        public async Task LoginAsync_WhenUserPasswordNotMatch_ShouldNotLoginIn()
        {
            //Arrange
            var loginUserData = AuthenticationBuilder.Create();

            var userRepository = AuthenticationMocks.UserRepository();
            var mapper = CommonMocks.Mapper();
            var logger = CommonMocks.Logger<AuthenticationService>();
            var refreshTokenRepository = AuthenticationMocks.RefreshTokenRepository();
            var jwtProvider = AuthenticationMocks.JwtProvider();
            var refreshTokenGenerator = AuthenticationMocks.RefreshTokenGenerator();
            var refreshTokenHasher = AuthenticationMocks.RefreshTokenHasher();
            var jwtSettings = AuthenticationMocks.JwtSettings();
            var unitOfWork = CommonMocks.UnitOfWork();
            var currentUserService = CommonMocks.CurrentUserService();
            var cookieService = AuthenticationMocks.CookieService();

            User user = CommonBuilder.CreateUser();

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("EZ12345EZ12345");

            userRepository.Setup(repo => repo.GetUserByEmailAsync(loginUserData.Email, false)).ReturnsAsync(user);

            var authenticationService = new AuthenticationService(userRepository.Object, mapper, logger.Object, refreshTokenRepository.Object, jwtProvider.Object,
            refreshTokenGenerator.Object, refreshTokenHasher.Object, jwtSettings, unitOfWork.Object, currentUserService.Object
            , cookieService.Object);


            //Act
            var resul = await authenticationService.LoginAsync(loginUserData);

            //Assert

            Assert.False(resul.IsSuccess);
            Assert.Null(resul.Data);

        }

        [Fact]
        public async Task RefreshAsync_WhenTokenIsValid_ShouldReturnNewTokens()
        {
            // Arrange
            var userRepository = AuthenticationMocks.UserRepository();
            var mapper = CommonMocks.Mapper();
            var logger = CommonMocks.Logger<AuthenticationService>();
            var refreshTokenRepository = AuthenticationMocks.RefreshTokenRepository();
            var jwtProvider = AuthenticationMocks.JwtProvider();
            var refreshTokenGenerator = AuthenticationMocks.RefreshTokenGenerator();
            var refreshTokenHasher = AuthenticationMocks.RefreshTokenHasher();
            var jwtSettings = AuthenticationMocks.JwtSettings();
            var unitOfWork = CommonMocks.UnitOfWork();
            var currentUserService = CommonMocks.CurrentUserService();
            var cookieService = AuthenticationMocks.CookieService();

            var user = CommonBuilder.CreateUser();
            var oldTokenString = "old-token-from-cookie";
            var oldTokenHash = "old-hash-123";

            var oldTokenEntity = new RefreshToken
            {
                TokenHash = oldTokenHash,
                UserId = user.Id,
                User = user,
                ExpiresAt = DateTime.UtcNow.AddDays(1), 
                RevokedAt = null 
            };

            var newAccessToken = "new-access-token-456";
            var newRefreshTokenString = "new-refresh-token-789";
            var newRefreshTokenHash = "new-hash-789";

            cookieService.Setup(c => c.GetRefreshToken()).Returns(oldTokenString);
            refreshTokenHasher.Setup(h => h.Hash(oldTokenString)).Returns(oldTokenHash);
            refreshTokenRepository.Setup(repo => repo.GetByTokenHashAsync(oldTokenHash, true)).ReturnsAsync(oldTokenEntity);

            jwtProvider.Setup(j => j.GenerateToken(user)).Returns(newAccessToken);
            refreshTokenGenerator.Setup(g => g.Generate()).Returns(newRefreshTokenString);
            refreshTokenHasher.Setup(h => h.Hash(newRefreshTokenString)).Returns(newRefreshTokenHash);

            refreshTokenRepository.Setup(repo => repo.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
            unitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            var authenticationService = new AuthenticationService(userRepository.Object, mapper, logger.Object, refreshTokenRepository.Object, jwtProvider.Object,
                refreshTokenGenerator.Object, refreshTokenHasher.Object, jwtSettings, unitOfWork.Object, currentUserService.Object, cookieService.Object);

            // Act
            var result = await authenticationService.RefreshAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newAccessToken, result.Data.AccessToken);
            Assert.Equal(newRefreshTokenString, result.Data.RefreshToken);

            Assert.NotNull(oldTokenEntity.RevokedAt);
            Assert.Equal(newRefreshTokenHash, oldTokenEntity.ReplacedByTokenHash);

            refreshTokenRepository.Verify(repo => repo.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
            unitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_WhenTokenIsValid_ShouldRevokeAndReturnSuccess()
        {
            // Arrange
            var userRepository = AuthenticationMocks.UserRepository();
            var mapper = CommonMocks.Mapper();
            var logger = CommonMocks.Logger<AuthenticationService>();
            var refreshTokenRepository = AuthenticationMocks.RefreshTokenRepository();
            var jwtProvider = AuthenticationMocks.JwtProvider();
            var refreshTokenGenerator = AuthenticationMocks.RefreshTokenGenerator();
            var refreshTokenHasher = AuthenticationMocks.RefreshTokenHasher();
            var jwtSettings = AuthenticationMocks.JwtSettings();
            var unitOfWork = CommonMocks.UnitOfWork();
            var currentUserService = CommonMocks.CurrentUserService();
            var cookieService = AuthenticationMocks.CookieService();

            var tokenString = "valid-token";
            var tokenHash = "hash-123";
            var tokenEntity = new RefreshToken
            {
                TokenHash = tokenHash,
                RevokedAt = null
            };

            cookieService.Setup(c => c.GetRefreshToken()).Returns(tokenString);
            refreshTokenHasher.Setup(h => h.Hash(tokenString)).Returns(tokenHash);
            refreshTokenRepository.Setup(repo => repo.GetByTokenHashAsync(tokenHash, true)).ReturnsAsync(tokenEntity);

            unitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            var authenticationService = new AuthenticationService(userRepository.Object, mapper, logger.Object, refreshTokenRepository.Object, jwtProvider.Object,
                refreshTokenGenerator.Object, refreshTokenHasher.Object, jwtSettings, unitOfWork.Object, currentUserService.Object, cookieService.Object);

            // Act
            var result = await authenticationService.LogoutAsync();

            // Assert
            Assert.True(result.IsSuccess);

            Assert.NotNull(tokenEntity.RevokedAt);

            unitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task LogoutFromAllDevicesAsync_WhenUserIsAuthenticated_ShouldRevokeAllTokens()
        {
            // Arrange
            var userRepository = AuthenticationMocks.UserRepository();
            var mapper = CommonMocks.Mapper();
            var logger = CommonMocks.Logger<AuthenticationService>();
            var refreshTokenRepository = AuthenticationMocks.RefreshTokenRepository();
            var jwtProvider = AuthenticationMocks.JwtProvider();
            var refreshTokenGenerator = AuthenticationMocks.RefreshTokenGenerator();
            var refreshTokenHasher = AuthenticationMocks.RefreshTokenHasher();
            var jwtSettings = AuthenticationMocks.JwtSettings();
            var unitOfWork = CommonMocks.UnitOfWork();
            var currentUserService = CommonMocks.CurrentUserService();
            var cookieService = AuthenticationMocks.CookieService();

            int currentUserId = 1;
            currentUserService.Setup(c => c.UserId).Returns(currentUserId);

            var activeTokens = new List<RefreshToken>
            {
              new RefreshToken { Id = 1, RevokedAt = null },
              new RefreshToken { Id = 2, RevokedAt = null }
             };

            refreshTokenRepository.Setup(repo => repo.GetAllActiveTokensByUserIdAsync(currentUserId, true)).ReturnsAsync(activeTokens);
            unitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            var authenticationService = new AuthenticationService(userRepository.Object, mapper, logger.Object, refreshTokenRepository.Object, jwtProvider.Object,
                refreshTokenGenerator.Object, refreshTokenHasher.Object, jwtSettings, unitOfWork.Object, currentUserService.Object, cookieService.Object);

            // Act
            var result = await authenticationService.LogoutFromAllDevicesAsync();

            // Assert
            Assert.True(result.IsSuccess);

            Assert.NotNull(activeTokens[0].RevokedAt);
            Assert.NotNull(activeTokens[1].RevokedAt);

            unitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }
    }
    
}
