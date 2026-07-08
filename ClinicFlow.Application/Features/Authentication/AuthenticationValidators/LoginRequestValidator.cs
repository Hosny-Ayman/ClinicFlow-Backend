using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Authentication.DTOs.Requests;
using FluentValidation;
namespace ClinicFlow.Application.Features.Authentication.AuthenticationValidators
{
    public class LoginRequestValidator: AbstractValidator<LoginDtoRequest>
    {

        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).EmailRule();
            RuleFor(x => x.Password).PasswordRule();

        }



    }
}
