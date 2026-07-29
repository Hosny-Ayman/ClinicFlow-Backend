using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Common.Authorization;
using ClinicFlow.Application.Features.Doctors;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [Authorize(policy:(Policies.ManageUsers))]
        [HttpPost]

        public async Task<IActionResult> Create(CreateDoctorDtoRequest docterDto)
        {
            var result = await _doctorService.CreateDoctorAsync(docterDto);

            return this.ToHttpResponse(result);
        }


    }
}
