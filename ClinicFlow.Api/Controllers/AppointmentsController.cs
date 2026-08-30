using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Features.Appointments;
using ClinicFlow.Application.Features.Appointments.DTOs;
using ClinicFlow.Application.Features.Appointments.DTOs.Requests;
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

        [Authorize(policy: nameof(PermissionEnum.AppointmentsViewAll))]
        [HttpPost("GetAllAppointment")]
        public async Task<IActionResult> GetAllAppointmentAsync([FromBody]AppointmentSearchDtoRequest request)
        {
            var result = await _appointmentService.GetAllAppointmentAsync(request);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.AppointmentsView))]
        [HttpGet("GetAppointmentDashboard")]
        public async Task<IActionResult> GetAppointmentDashboard(DateOnly date)
        {
            var result = await _appointmentService.GetAppointmentDashboardAsync(date);

            return this.ToHttpResponse(result);
        }

        [Authorize(policy: nameof(PermissionEnum.AppointmentsUpdate))]
        [HttpPut("UpdateAppointmentStatus")]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, AppointmentStatusEnum status)
        {
            var result = await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, status);

            return this.ToHttpResponse(result);
        }


    }
}
