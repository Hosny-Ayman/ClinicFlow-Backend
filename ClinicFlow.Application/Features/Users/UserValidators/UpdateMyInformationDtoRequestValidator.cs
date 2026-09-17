using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.Users.UserValidators
{
    public class UpdateMyInformationDtoRequestValidator : AbstractValidator<UpdateMyInformationDtoRequest>
    {
        public UpdateMyInformationDtoRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("FirstName Should Not Be Empty")
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("FirstName cannot be empty or whitespace only")
                .MaximumLength(100).WithMessage("FirstName is too long");

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("LastName Should Not Be Empty")
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("LastName cannot be empty or whitespace only")
                .MaximumLength(100).WithMessage("LastName is too long");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email Should Not Be Empty")
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Email cannot be empty or whitespace only")
                .EmailRule();

            RuleFor(x => x.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Phone Should Not Be Empty")
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Phone cannot be empty or whitespace only")
                .PhoneRule();
        }
    }
}
