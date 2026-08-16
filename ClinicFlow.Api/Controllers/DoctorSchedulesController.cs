using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Features.Doctors;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Application.Features.DoctorSchedules;
using ClinicFlow.Application.Features.DoctorSchedules.DTOs.Requests;
using ClinicFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorSchedulesController : ControllerBase
    {
        private readonly DoctorScheduleService _doctorScheduleService;

        public DoctorSchedulesController(DoctorScheduleService doctorScheduleService)
        {
            _doctorScheduleService = doctorScheduleService;
        }

        [Authorize(policy: nameof(PermissionEnum.DoctorSchedulesView))]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAllDoctorSchedules(int userId)
        {
            var result = await _doctorScheduleService.GetAllDoctorSchedulesAsync(userId);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.DoctorSchedulesUpdate))]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateSchedulesInsideProjectAsync([FromRoute]int userId, [FromBody] List<UpdateAndGetDoctorScheduleDtoRequest> request)
        {
            var result = await _doctorScheduleService.UpdateSchedulesInsideProjectAsync(request, userId);

            return this.ToHttpResponse(result);
        }
    }
}
