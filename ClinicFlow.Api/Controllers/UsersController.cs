using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Common.Authorization;
using ClinicFlow.Application.Features.Doctors;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Application.Features.Users;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {

            var result = await _userService.GetCurrentUserAsync();

            return this.ToHttpResponse(result);

        }


        [Authorize(policy: (Policies.ManageUsers))]
        [HttpPost("Receptionist")]
        public async Task<IActionResult> Receptionist(CreateUserDtoRequest userDto)
        {
            var result = await _userService.CreateReceptionistAsync(userDto);

            return this.ToHttpResponse(result);
        }


    }
}
