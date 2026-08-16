using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.DoctorSchedules.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.DoctorSchedules.DoctorScheduleValidators
{
    public class UpdateAndGetDoctorScheduleDtoRequestValidator:AbstractValidator<UpdateAndGetDoctorScheduleDtoRequest>
    {

        public UpdateAndGetDoctorScheduleDtoRequestValidator()
        {

            RuleFor(x => x.Id).RequiredRule("Id");
            RuleFor(x => x.DayOfWeek).RequiredRule("DayOfWeek");
            RuleFor(x => x.StartTime).RequiredRule("StartTime");
            RuleFor(x => x.EndTime).RequiredRule("EndTime");
            RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime).WithMessage("EndTime must be greater than StartTime.");
        }

    }
}
