using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Common.Authorization;
using ClinicFlow.Application.Features.ClinicSetups;
using ClinicFlow.Application.Features.ClinicSetups.DTOs.Requests;
using ClinicFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicSetupsController : ControllerBase
    {

        private readonly ClinicSetupService _clinicSetupService;

        public ClinicSetupsController(ClinicSetupService clinicSetupService)
        {
            _clinicSetupService = clinicSetupService;
        }

        [Authorize(policy: nameof(PermissionEnum.ClinicsCreate))]
        [HttpPost]
        public async Task<IActionResult> Create(CreateAndEditClinicSetupDtoRequest request)
        {
            var result = await _clinicSetupService.CreateClinicSetupAsync(request);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.ClinicsView))]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _clinicSetupService.GetClinicSetupStatusAsync();

            return this.ToHttpResponse(result);
        }


        [Authorize(policy: nameof(PermissionEnum.ClinicsUpdate))]
        [HttpPut]
        public async Task<IActionResult> Update(CreateAndEditClinicSetupDtoRequest request)
        {
            var result = await _clinicSetupService.UpdateClinicSetupAsync(request);

            return this.ToHttpResponse(result);
        }

        


    }
}
