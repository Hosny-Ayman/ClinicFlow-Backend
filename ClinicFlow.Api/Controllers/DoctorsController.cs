using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Common.Authorization;
using ClinicFlow.Application.Features.Doctors;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {

        private DoctorService _doctorService;

        public DoctorsController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [Authorize(policy:nameof(PermissionEnum.DoctorsCreate))]
        [HttpPost("steps")]
        public async Task<IActionResult> CreateDoctorSteps([FromForm] CreateAndEditDoctorDtoRequest docterDto)
        {
            var result = await _doctorService.CreateDoctorStepsAsync(docterDto);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.DoctorsCreate))]
        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromForm] CreateAndEditDoctorWithUserDtoRequest Request)
        {
            var result = await _doctorService.CreateDoctorAsync(Request.Doctor, Request.User);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.DoctorsView))]
        [HttpGet("{doctorId:int}")]
        public async Task<IActionResult> GetDoctor(int doctorId)
        {
            var result = await _doctorService.GetDoctorFullInforamtionByIdAsync(doctorId);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.DoctorsUpdate))]
        [HttpPut]
        public async Task<IActionResult> UpdateDoctor([FromForm] UpdateDoctorFullInforamtionDtoRequest request)
        {
            var result = await _doctorService.UpdateDoctorAsync(request.User, request.Doctor);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.DoctorsViewAll))]
        [HttpPost("GetAllDoctors")]
        public async Task<IActionResult> GetAllDoctorsInformations([FromBody]DoctorSearchDtoRequest request)
        {
            var result = await _doctorService.GetAllDoctorsInformationsAsync(request);

            return this.ToHttpResponse(result);
        }


    }
}
