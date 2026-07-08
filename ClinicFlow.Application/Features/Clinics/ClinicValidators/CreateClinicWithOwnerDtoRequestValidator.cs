using FluentValidation;
using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Clinics.DTOs.Requests;

namespace ClinicFlow.Application.Features.Clinics.ClinicValidators
{
    public class CreateClinicWithOwnerDtoRequestValidator : AbstractValidator<CreateClinicWithOwnerDtoRequest>
    {
        public CreateClinicWithOwnerDtoRequestValidator()
        {

            RuleFor(x => x.Clinic.Name).NameRule();

            RuleFor(x => x.Clinic.Phone).PhoneRule();

            RuleFor(x => x.Clinic.Email).EmailRule();

            RuleFor(x => x.Clinic.Address).AddressRule();

            RuleFor(x => x.User.FirstName).FirtsNameRule();

            RuleFor(x => x.User.LastName).LastNameRule();

            RuleFor(x => x.User.Email).EmailRule();

            RuleFor(x => x.User.PhoneNumber).PhoneRule();

            RuleFor(x => x.User.Password).PasswordRule();

        }



    }
}
