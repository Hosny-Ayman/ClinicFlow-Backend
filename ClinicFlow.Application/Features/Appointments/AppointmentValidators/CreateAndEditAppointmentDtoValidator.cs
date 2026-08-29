using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Appointments.DTOs;
using ClinicFlow.Domain.Entities;
using FluentValidation;

namespace ClinicFlow.Application.Features.Appointments.AppointmentValidators
{
    public class CreateAndEditAppointmentDtoValidator:AbstractValidator<CreateAndEditAppointmentDto>
    {

        public CreateAndEditAppointmentDtoValidator()
        {
            RuleFor(x => x.Id).NotNull()
           .WithMessage("Id is required for update.")
           .When(x => x.Id.HasValue);

            RuleFor(x => x.PatientId).RequiredRule("PatientId");

            RuleFor(x => x.DoctorId).RequiredRule("DoctorId");
            RuleFor(x => x.AppointmentDate).RequiredRule("AppointmentDate");
            RuleFor(x => x.StartTime).RequiredRule("StartTime");
            RuleFor(x => x.Status).RequiredRule("Status");

            RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime).WithMessage("EndTime must be greater than StartTime.");


        }


    }
}
