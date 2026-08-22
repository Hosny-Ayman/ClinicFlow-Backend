using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Features.DoctorVacations;
using ClinicFlow.Application.Features.DoctorVacations.DTOs;
using ClinicFlow.Application.Features.DoctorVacations.DTOs.Requests;
using ClinicFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorVacationsController : ControllerBase
    {

        private readonly DoctorVacationService _doctorVacationService;

        public DoctorVacationsController(DoctorVacationService doctorVacationService)
        {
            _doctorVacationService = doctorVacationService;
        }

        [Authorize(policy: nameof(PermissionEnum.DoctorVacationsCreate))]
        [HttpPost()]
        public async Task<IActionResult> CreateDoctorVacation(Get_Create_Update_DoctorVacationDto respons)
        {
            var result = await _doctorVacationService.CreateDoctorVacationAsyn(respons);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.DoctorVacationsView))]
        [HttpGet("{userId}/{vacationId}")]
        public async Task<IActionResult> GetDoctorVacationInformation(int userId,int vacationId)
        {
            var result = await _doctorVacationService.GetDoctorVacationInformationAsync(userId, vacationId);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.DoctorVacationsUpdate))]
        [HttpPut]
        public async Task<IActionResult> UpdateDoctorVacation(Get_Create_Update_DoctorVacationDto respons)
        {
            var result = await _doctorVacationService.UpdateDoctorVacationAsyn(respons);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.DoctorVacationsViewAll))]
        [HttpPost("GetAllDoctorVacation")]
        public async Task<IActionResult> GetAllDoctorVacationInformation([FromBody]DoctorVacationSearchDtoRequest respons)
        {
            var result = await _doctorVacationService.GetAllDoctorVacationInformationAsync(respons);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.DoctorVacationsViewAll))]
        [HttpGet("GetDoctorVacationDashboardInformation")]
        public async Task<IActionResult> GetDoctorVacationDashboardInformation()
        {
            var result = await _doctorVacationService.GetDoctorVacationDashboardInformationAsync();

            return this.ToHttpResponse(result);
        }


    }
}
