using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Patients.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.Patients.PatientValidators
{
    public class PatientSearchDtoRequestValidator : AbstractValidator<PatientSearchDtoRequest>
    {

        public PatientSearchDtoRequestValidator()
        {
            RuleFor(x => x.PageNumber).RequiredRule("PageNumber");
            RuleFor(x => x.PageSize).RequiredRule("PageSize");
        }

    }
}
