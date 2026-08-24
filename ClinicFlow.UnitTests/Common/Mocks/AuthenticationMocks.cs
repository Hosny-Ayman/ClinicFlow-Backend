using ClinicFlow.Application.Common.Authentication;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Moq;

namespace ClinicFlow.UnitTests.Common.Mocks
{
    public static class AuthenticationMocks
    {

        public static Mock<IUserRepository> UserRepository() => new Mock<IUserRepository>();

        public static Mock<IDoctorRepository> DoctorRepository() => new Mock<IDoctorRepository>();

        public static Mock<IRefreshTokenRepository> RefreshTokenRepository() => new Mock<IRefreshTokenRepository>();

        public static Mock<IRefreshTokenGenerator> RefreshTokenGenerator() => new Mock<IRefreshTokenGenerator>();

        public static Mock<IJwtProvider> JwtProvider() => new Mock<IJwtProvider>();

        public static Mock<IRefreshTokenHasher> RefreshTokenHasher() => new Mock<IRefreshTokenHasher>();

        public static IOptions<JwtSettings> JwtSettings()
        {
            return Options.Create(new JwtSettings
            {
                Key = "test-secret-key",
                Issuer = "test-issuer",
                Audience = "test-audience",
                DurationInMinutes = 30,
                RefreshTokenDurationInDays = 30
            });
        }
           

        public static Mock<ICookieService> CookieService() => new Mock<ICookieService>();

    }
}
