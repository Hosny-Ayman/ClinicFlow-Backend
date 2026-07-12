using ClinicFlow.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ClinicFlow.Infrastructure.Authentication
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public int? UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                return int.TryParse(value, out var id)? id : null;
            }
        }

        public string? Email =>_httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

        public int? ClinicId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User.FindFirst("ClinicId")?.Value;

                return int.TryParse(value, out var clinicId)? clinicId : null;
            }
        }
    }
}
