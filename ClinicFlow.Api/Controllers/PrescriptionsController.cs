using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Features.Prescriptions;
using ClinicFlow.Application.Features.Prescriptions.DTOs.Requests;
using ClinicFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly PrescriptionService _prescriptionService;

        public PrescriptionsController(PrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        [Authorize(policy: nameof(PermissionEnum.PrescriptionsCreate))]
        [HttpPost]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionDtoRequest request)
        {
            var result = await _prescriptionService.CreatePrescriptionAsync(request);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.PrescriptionsView))]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPrescriptionById(int id)
        {
            var result = await _prescriptionService.GetPrescriptionByIdAsync(id);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.PrescriptionsView))]
        [HttpGet("ByMedicalRecord/{medicalRecordId:int}")]
        public async Task<IActionResult> GetPrescriptionByMedicalRecordId(int medicalRecordId)
        {
            var result = await _prescriptionService.GetPrescriptionByMedicalRecordIdAsync(medicalRecordId);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.PrescriptionsUpdate))]
        [HttpPut]
        public async Task<IActionResult> UpdatePrescription([FromBody] UpdatePrescriptionDtoRequest request)
        {
            var result = await _prescriptionService.UpdatePrescriptionAsync(request);

            return this.ToHttpResponse(result);
        }
    }
}
