using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.Users.UserValidators
{
    public class CreateUserValidator:AbstractValidator<CreateUserDtoRequest>
    {

        public CreateUserValidator()
        {

            RuleFor(x => x.FirstName).FirtsNameRule();
            RuleFor(x => x.LastName).LastNameRule();
            RuleFor(x => x.Email).EmailRule();
            RuleFor(x => x.PhoneNumber).PhoneRule();
            RuleFor(x => x.Password).PasswordRule();

        }

    }
}
