using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.Doctors.DoctorValidators
{
    public class DoctorSearchDtoRequestValidator :AbstractValidator<DoctorSearchDtoRequest>
    {

        public DoctorSearchDtoRequestValidator()
        {
            RuleFor(x => x.PageNumber).RequiredRule("PageNumber");
            RuleFor(x => x.PageSize).RequiredRule("PageSize");
        }

    }
}
