using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicFlow.Application.Features.Users.UserValidators
{
    public class UpdateUserInformationDtoRequestValidator:AbstractValidator<UpdateUserInformationDtoRequest>
    {

        public UpdateUserInformationDtoRequestValidator()
        {
            RuleFor(x => x.Id).RequiredRule("Id");
            RuleFor(x => x.FirstName).FirtsNameRule();
            RuleFor(x => x.LastName).FirtsNameRule();
            RuleFor(x => x.Email).EmailRule();
            RuleFor(x => x.Password).PasswordRule().When(x => !string.IsNullOrWhiteSpace(x.Password));
            RuleFor(x => x.PhoneNumber).PhoneRule();
            

            

        }
    }
}
