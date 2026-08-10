using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Common.Authorization;
using ClinicFlow.Application.Features.Clinics;
using ClinicFlow.Application.Features.Clinics.DTOs.Requests;
using ClinicFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers
{
    //[Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicsController : ControllerBase
    {
        private readonly ClinicService _clinicService;
        public ClinicsController(ClinicService ClinicService)
        {
            _clinicService = ClinicService;
        }

        
        [AllowAnonymous]
        [HttpPost]
        public async Task <IActionResult> Create([FromForm]CreateClinicWithOwnerDtoRequest requst)
        {
            var resul = await _clinicService.CreateClinicAsync(requst.Clinic, requst.User);

            return this.ToHttpResponse(resul);
        }

        [Authorize(policy:nameof(PermissionEnum.ClinicsUpdate))]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm]CreateAndEditClinicDtoRequest request)
        {
            var resul = await _clinicService.UpdateClinicAsync(request);

            return this.ToHttpResponse(resul);
        }

        [Authorize(policy: nameof(PermissionEnum.ClinicsView))]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var resul = await _clinicService.GetClinicAsync();

            return this.ToHttpResponse(resul);
        }


    }
}
