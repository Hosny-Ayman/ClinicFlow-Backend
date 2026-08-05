using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Common.Authorization;
using ClinicFlow.Application.Features.ClinicWorkingHours;
using ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicWorkingHoursController : ControllerBase
    {

        private readonly ClinicWorkingHoursService _clinicWorkingHoursService;

        public ClinicWorkingHoursController(ClinicWorkingHoursService clinicWorkingHoursService)
        {
            _clinicWorkingHoursService = clinicWorkingHoursService;
        }


        [Authorize(policy: (Policies.ManageUsers))]
        [HttpPost]
        public async Task<IActionResult> Create(List<CreateClinicWorkingHourDtoRequest> DaysDto)
        {
            var result = await _clinicWorkingHoursService.CreateWorkingHoursAndDaysAsync(DaysDto);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: (Policies.ManageUsers))]
        [HttpPut]
        public async Task<IActionResult> Update(List<UpdateClinicWorkingHoursAndDaysDtoRequest> request)
        {
            var result = await _clinicWorkingHoursService.UpdateWorkingHoursAndDaysAsync(request);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: (Policies.ManageUsers))]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _clinicWorkingHoursService.GetAllWorkingHoursAndDaysAsync();

            return this.ToHttpResponse(result);
        }


    }
}
