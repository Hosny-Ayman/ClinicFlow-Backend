using ClinicFlow.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ClinicFlow.Infrastructure.Authentication
{
    internal class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetRefreshToken()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies["RefreshToken"];
        }
    }
}
