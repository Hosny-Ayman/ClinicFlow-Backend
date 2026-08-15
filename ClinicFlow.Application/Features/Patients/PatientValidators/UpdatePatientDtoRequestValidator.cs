using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Patients.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.Patients.PatientValidators
{
    public class UpdatePatientDtoRequestValidator : AbstractValidator<UpdatePatientDtoRequest>
    {

        public UpdatePatientDtoRequestValidator()
        {
            RuleFor(x => x.Id).RequiredRule("Id");
            RuleFor(x => x.FirstName).FirtsNameRule();
            RuleFor(x => x.LastName).LastNameRule();
            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("DateOfBirth Is Required")
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("DateOfBirth Must Be In The Past");
            RuleFor(x => x.Gender).RequiredRule("Gender");
        }

    }
}
