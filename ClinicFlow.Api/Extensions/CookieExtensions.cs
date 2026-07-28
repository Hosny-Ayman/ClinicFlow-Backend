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
                    SameSite = SameSiteMode.None,
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
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(jwtSettings.RefreshTokenDurationInDays)
                });
        }

        public static void DeleteAccessToken(this HttpResponse response)
        {
            response.Cookies.Delete("AccessToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });
        }

        public static void DeleteRefreshToken(this HttpResponse response)
        {
            response.Cookies.Delete("RefreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });
        }
    }
}
