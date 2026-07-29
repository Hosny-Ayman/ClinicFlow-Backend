using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace ClinicFlow.Application.Common.ValidationRules
{
    public static class FileRules
    {
        public static IRuleBuilderOptions<T, IFormFile?> ImageRule<T>(this IRuleBuilder<T, IFormFile?> rule,string name)
        {
            return rule
                .Must(file => file == null || file.Length > 0)
                .WithMessage($"{name} should not be empty")

                .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
                .WithMessage($"{name} size should not exceed 5 MB")

                .Must(file =>
                    file == null ||
                    file.ContentType == "image/jpeg" ||
                    file.ContentType == "image/png" ||
                    file.ContentType == "image/webp")
                .WithMessage($"{name} must be a valid image");
        }


    }
}
