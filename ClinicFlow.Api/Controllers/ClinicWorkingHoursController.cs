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


    }
}
