using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Common.Authentication;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Authentication;
using ClinicFlow.Application.Features.Authentication.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ClinicFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly JwtSettings _jwtSettings;

        public AuthenticationController(AuthenticationService authenticationService, IOptions<JwtSettings> jwtOptions)
        {
            _authenticationService = authenticationService;
            _jwtSettings = jwtOptions.Value;
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginDtoRequest request)
        {
            var result = await _authenticationService.LoginAsync(request);

            if (result.IsSuccess)
            {

                Response.SetAccessToken(result.Data.AccessToken, _jwtSettings);

                Response.SetRefreshToken(result.Data.RefreshToken, _jwtSettings);

                return Ok(OperationResult<object>.Success(result.Data.User));
            }

            return this.ToHttpResponse(result);

        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["RefreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized();
            }

            var result = await _authenticationService.RefreshAsync(refreshToken);

            if(result.IsSuccess)
            {

                Response.SetAccessToken(result.Data.AccessToken, _jwtSettings);
                Response.SetRefreshToken(result.Data.RefreshToken, _jwtSettings);

                return Ok(OperationResult<object>.Success("Token refreshed successfully"));

            }

            return this.ToHttpResponse(result);

        }
    }
}
