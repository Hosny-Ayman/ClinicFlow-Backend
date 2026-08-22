using FluentValidation;

namespace ClinicFlow.Application.Common.ValidationRules
{
    public static class DoctorRules
    {

        public static IRuleBuilderOptions<T, int> RequiredRule<T>(this IRuleBuilder<T, int> rule,string name)
        {
            return rule
                .GreaterThan(0)
                .WithMessage($"{name} Should Be Greater Than Zero");
        }

        public static IRuleBuilderOptions<T, decimal> RequiredRule<T>(this IRuleBuilder<T, decimal> rule, string name)
        {
            return rule
                .GreaterThan(0)
                .WithMessage($"{name} Should Be Greater Than Zero");
        }

        public static IRuleBuilderOptions<T, TEnum> RequiredRule<T, TEnum>(this IRuleBuilder<T, TEnum> rule,string name)where TEnum : struct, Enum
        {
            return rule
                .IsInEnum()
                .NotEqual(default(TEnum))
                .WithMessage($"{name} Should Be Selected");
        }

        public static IRuleBuilderOptions<T, TimeOnly> RequiredRule<T>(this IRuleBuilder<T, TimeOnly> rule, string name)
        {
            return rule
                .NotNull()
                .WithMessage($"{name} Is Required");
        }

        public static IRuleBuilderOptions<T, DateOnly> RequiredRule<T>(this IRuleBuilder<T, DateOnly> rule, string name)
        {
            return rule
                .NotNull()
                .WithMessage($"{name} Is Required");
        }



    }
}
