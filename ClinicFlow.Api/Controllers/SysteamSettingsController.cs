using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Features.SysteamSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SysteamSettingsController : ControllerBase
    {
        private readonly SysteamSettingService _systeamSettingService;
        public SysteamSettingsController(SysteamSettingService systeamSettingService)
        {
            _systeamSettingService = systeamSettingService;
        }
        [AllowAnonymous]
        [HttpGet("Image")]
        public async Task <IActionResult> SytemImage(string imageKey)
        {
            var result = await _systeamSettingService.GetSystemImageAsync(imageKey);

            return this.ToHttpResponse(result);
        }


    }
}
