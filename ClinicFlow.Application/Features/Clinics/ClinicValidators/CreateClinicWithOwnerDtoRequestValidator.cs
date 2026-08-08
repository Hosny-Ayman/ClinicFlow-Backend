using FluentValidation;
using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Clinics.DTOs.Requests;
using ClinicFlow.Application.Features.Users.UserValidators;

namespace ClinicFlow.Application.Features.Clinics.ClinicValidators
{
    public class CreateClinicWithOwnerDtoRequestValidator : AbstractValidator<CreateClinicWithOwnerDtoRequest>
    {
        public CreateClinicWithOwnerDtoRequestValidator()
        {
            RuleFor(x => x.Clinic).SetValidator(new CreateAndEditClinicDtoRequestValidator());

            RuleFor(x => x.User).SetValidator(new CreateUserDtoRequestValidator());

        }



    }
}
