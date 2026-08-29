using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Features.Appointments;
using ClinicFlow.Application.Features.Appointments.DTOs;
using ClinicFlow.Application.Features.Appointments.DTOs.Requests;
using ClinicFlow.Application.Features.Doctors;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;
        public AppointmentsController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [Authorize(policy: nameof(PermissionEnum.AppointmentsCreate))]
        [HttpPost("CreateAppointment")]
        public async Task<IActionResult> CreateAppointment(CreateAndEditAppointmentDto request)
        {
            var result = await _appointmentService.AddAppointmentAsync(request);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.AppointmentsView))]
        [HttpGet("GetDoctorAvailableSlots")]
        public async Task<IActionResult> GetDoctorAvailableSlots([FromQuery]DoctorAvailableSlotsDtoRequest request)
        {
            var result = await _appointmentService.GetDoctorAvailableSlotsByDateAsync(request);

            return this.ToHttpResponse(result);
        }


    }
}
