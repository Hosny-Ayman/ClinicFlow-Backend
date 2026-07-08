using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicFlow.Application.Common.ValidationRules
{
    public static class ClinicRules
    {

        public static IRuleBuilderOptions<T,string> NameRule<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
               .NotEmpty().WithMessage("Name Should Not Be Empty")
               .MaximumLength(150).WithMessage("Name is too long");
        }


        public static IRuleBuilderOptions<T, string> PhoneRule<T>(this IRuleBuilder<T, string> rule)
        {
            return rule.NotEmpty().WithMessage("Phone Should Not Be Empty")

                        .Length(11, 20)
                        .WithMessage("Phone Should be between 11 and 20 characters")

                        .Matches(@"^\d+$")
                        .WithMessage("Phone Should Contain Numbers Only");
        }

        public static IRuleBuilderOptions<T, string> EmailRule<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .NotEmpty().WithMessage("Email Should Not Be Empty")
                .EmailAddress().WithMessage("Invalid Email Format");
        }

        public static IRuleBuilderOptions<T, string> AddressRule<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .NotEmpty().WithMessage("Address Should Not Be Empty")
                .MaximumLength(300).WithMessage("Address is too long");
        }



    }
}
