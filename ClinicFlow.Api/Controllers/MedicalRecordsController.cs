using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Features.MedicalRecords;
using ClinicFlow.Application.Features.MedicalRecords.DTOs.Requests;
using ClinicFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly MedicalRecordService _medicalRecordService;

        public MedicalRecordsController(MedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [Authorize(policy: nameof(PermissionEnum.MedicalRecordsCreate))]
        [HttpPost]
        public async Task<IActionResult> CreateMedicalRecord([FromBody] CreateMedicalRecordDtoRequest request)
        {
            var result = await _medicalRecordService.CreateMedicalRecordAsync(request);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.MedicalRecordsView))]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMedicalRecordById(int id)
        {
            var result = await _medicalRecordService.GetMedicalRecordByIdAsync(id);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.MedicalRecordsView))]
        [HttpGet("ByAppointment/{appointmentId:int}")]
        public async Task<IActionResult> GetMedicalRecordByAppointmentId(int appointmentId)
        {
            var result = await _medicalRecordService.GetMedicalRecordByAppointmentIdAsync(appointmentId);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.MedicalRecordsUpdate))]
        [HttpPut]
        public async Task<IActionResult> UpdateMedicalRecord([FromBody] UpdateMedicalRecordDtoRequest request)
        {
            var result = await _medicalRecordService.UpdateMedicalRecordAsync(request);

            return this.ToHttpResponse(result);
        }
    }
}
