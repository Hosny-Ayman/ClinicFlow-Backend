using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Appointments.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.Appointments.AppointmentValidators
{
    public class DoctorAvailableSlotsDtoRequestValidator: AbstractValidator<DoctorAvailableSlotsDtoRequest>
    {

        public DoctorAvailableSlotsDtoRequestValidator()
        {
            RuleFor(x => x.doctorId).RequiredRule("doctorId");
            RuleFor(x => x.appointmentDate).RequiredRule("appointmentDate");
        }

    }
}
