using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Common.Authorization;
using ClinicFlow.Application.Features.Clinics;
using ClinicFlow.Application.Features.Clinics.DTOs.Requests;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [Authorize(policy:Policies.ManageDoctors)]
        [HttpPost]
        public async Task <IActionResult> Create(CreateClinicWithOwnerDtoRequest requst)
        {
            var resul = await _clinicService.CreateClinicAsync(requst.Clinic, requst.User);

            return this.ToHttpResponse(resul);
        }


    }
}
