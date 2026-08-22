using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.DoctorVacations.DTOs;
using FluentValidation;

namespace ClinicFlow.Application.Features.DoctorVacations.DoctorVacationValidators
{
    public class Get_Create_Update_DoctorVacationValidatorDto : AbstractValidator<Get_Create_Update_DoctorVacationDto>
    {

        public Get_Create_Update_DoctorVacationValidatorDto()
        {

            RuleFor(x => x.Id).NotNull()
            .WithMessage("Id is required for update.")
            .When(x => x.Id.HasValue);

            RuleFor(x => x.UserId).RequiredRule("UserId");

            RuleFor(x => x.StartDate).RequiredRule("StartDate");

            RuleFor(x => x.EndDate)
                .RequiredRule("EndDate")
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("EndDate must be greater than or equal to StartDate.");
        }

    }
}
