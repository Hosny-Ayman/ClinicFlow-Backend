using FluentValidation;

namespace ClinicFlow.Application.Common.ValidationRules
{
    public static class UserRules
    {

        public static IRuleBuilderOptions <T,string> FirtsNameRule<T> (this IRuleBuilder<T, string> rule)
        {
            return rule
               .NotEmpty().WithMessage("FirstName Should Not Be Empty")
               .MaximumLength(100).WithMessage("FirstName is too long");
        }

        public static IRuleBuilderOptions<T, string> LastNameRule<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
               .NotEmpty().WithMessage("LastName Should Not Be Empty")
               .MaximumLength(100).WithMessage("LastName is too long");
        }

        public static IRuleBuilderOptions<T, string> PasswordRule<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
               .NotEmpty().WithMessage("Password Should Not Be Empty")
               .MinimumLength(8)
               .WithMessage("Password must be at least 8 characters.")
               .MaximumLength(100)
               .WithMessage("Password cannot exceed 100 characters.");
            ;
              
        }

        public static IRuleBuilderOptions<T, string> ProFileImageRule<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
               .NotEmpty().WithMessage("ProFileImage Should Not Be Empty")
               .MaximumLength(500).WithMessage("ProFileImage Link is too long");
        }


    }
}
