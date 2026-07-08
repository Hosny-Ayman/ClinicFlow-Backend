using FluentValidation;
using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Clinics.DTOs.Requests;

namespace ClinicFlow.Application.Features.Clinics.ClinicValidators
{
    public class CreateClinicDtoRequestValidator: AbstractValidator<CreateClinicDtoRequest>
    {
        public CreateClinicDtoRequestValidator()
        {

            RuleFor(x => x.Name).NameRule();


            RuleFor(x => x.Phone).PhoneRule();
              

            RuleFor(x => x.Email).EmailRule();
               

            RuleFor(x => x.Address).AddressRule();
             
        }



    }
}
