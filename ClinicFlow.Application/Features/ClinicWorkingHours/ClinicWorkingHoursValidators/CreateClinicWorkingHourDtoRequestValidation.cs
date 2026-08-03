using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.ClinicWorkingHours.ClinicWorkingHoursValidators
{
    public class CreateClinicWorkingHourDtoRequestValidation:AbstractValidator<CreateClinicWorkingHourDtoRequest>
    {

        public CreateClinicWorkingHourDtoRequestValidation()
        {
            RuleFor(x => x.ClinicId).RequiredRule("ClinicId");
            RuleFor(x => x.Day).RequiredRule("Day");
            RuleFor(x => x.OpenTime).RequiredRule("OpenTime");
            RuleFor(x => x.CloseTime).RequiredRule("CloseTime");
        }



    }
}
