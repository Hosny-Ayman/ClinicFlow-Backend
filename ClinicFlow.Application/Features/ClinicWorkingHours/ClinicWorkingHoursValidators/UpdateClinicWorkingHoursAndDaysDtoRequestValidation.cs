using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.ClinicWorkingHours.ClinicWorkingHoursValidators
{
    public class UpdateClinicWorkingHoursAndDaysDtoRequestValidation:AbstractValidator<UpdateClinicWorkingHoursAndDaysDtoRequest>
    {

        public UpdateClinicWorkingHoursAndDaysDtoRequestValidation()
        {
            RuleFor(x => x.Id).RequiredRule("Id");
            RuleFor(x => x.Day).RequiredRule("Day");
            RuleFor(x => x.OpenTime).RequiredRule("OpenTime");
            RuleFor(x => x.CloseTime).RequiredRule("CloseTime");
            RuleFor(x => x.AppointmentDurationInMinutes).RequiredRule("AppointmentDurationInMinutes");

        }


    }
}
