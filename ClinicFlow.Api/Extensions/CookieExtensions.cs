using ClinicFlow.Application.Common.Authentication;

namespace ClinicFlow.Api.Extensions
{
    public static class CookieExtensions
    {

        public static void SetAccessToken(this HttpResponse response,string accessToken,JwtSettings jwtSettings)
        {
            response.Cookies.Append("AccessToken",accessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(jwtSettings.DurationInMinutes)
                });
        }

        public static void SetRefreshToken(this HttpResponse response,string refreshToken,JwtSettings jwtSettings)
        {
            response.Cookies.Append(
                "RefreshToken",
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(jwtSettings.RefreshTokenDurationInDays)
                });
        }
    }
}
