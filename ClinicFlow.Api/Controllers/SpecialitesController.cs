using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Common.Authorization;
using ClinicFlow.Application.Features.Specialties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialitesController : ControllerBase
    {
        private readonly SpecialityService _specialityService;
        public SpecialitesController(SpecialityService specialityService)
        {

            _specialityService = specialityService;


        }


        [Authorize(policy: (Policies.ManageDoctors))]
        [HttpGet("")]
        public async Task<IActionResult> GetAllSpecialites()
        {
            var result = await _specialityService.GetAllSpecialityAsync();

            return this.ToHttpResponse(result);
        }

    }
}
