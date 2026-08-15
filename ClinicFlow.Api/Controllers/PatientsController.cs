using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Features.Patients;
using ClinicFlow.Application.Features.Patients.DTOs.Requests;
using ClinicFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly PatientService _patientService;

        public PatientsController(PatientService patientService)
        {
            _patientService = patientService;
        }

        [Authorize(policy: nameof(PermissionEnum.PatientsCreate))]
        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDtoRequest request)
        {
            var result = await _patientService.CreatePatientAsync(request);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.PatientsView))]
        [HttpGet("{patientId:int}")]
        public async Task<IActionResult> GetPatient(int patientId)
        {
            var result = await _patientService.GetPatientByIdAsync(patientId);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.PatientsUpdate))]
        [HttpPut]
        public async Task<IActionResult> UpdatePatient([FromBody] UpdatePatientDtoRequest request)
        {
            var result = await _patientService.UpdatePatientAsync(request);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.PatientsViewAll))]
        [HttpPost("GetAllPatients")]
        public async Task<IActionResult> GetAllPatients([FromBody] PatientSearchDtoRequest request)
        {
            var result = await _patientService.GetAllPatientsAsync(request);

            return this.ToHttpResponse(result);
        }
    }
}
