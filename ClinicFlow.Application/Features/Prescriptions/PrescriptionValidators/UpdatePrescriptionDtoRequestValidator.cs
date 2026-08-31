using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Prescriptions.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.Prescriptions.PrescriptionValidators
{
    public class UpdatePrescriptionItemDtoRequestValidator : AbstractValidator<UpdatePrescriptionItemDtoRequest>
    {
        public UpdatePrescriptionItemDtoRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0).WithMessage("Id must be greater than or equal to 0.");

            RuleFor(x => x.MedicationName)
                .RequiredRule("MedicationName")
                .MaximumLength(200).WithMessage("MedicationName must not exceed 200 characters.");

            RuleFor(x => x.Dosage)
                .RequiredRule("Dosage")
                .MaximumLength(100).WithMessage("Dosage must not exceed 100 characters.");

            RuleFor(x => x.Frequency)
                .RequiredRule("Frequency")
                .MaximumLength(100).WithMessage("Frequency must not exceed 100 characters.");

            RuleFor(x => x.Duration)
                .RequiredRule("Duration")
                .MaximumLength(100).WithMessage("Duration must not exceed 100 characters.");

            RuleFor(x => x.Instructions)
                .MaximumLength(1000).WithMessage("Instructions must not exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Instructions));
        }
    }

    public class UpdatePrescriptionDtoRequestValidator : AbstractValidator<UpdatePrescriptionDtoRequest>
    {
        public UpdatePrescriptionDtoRequestValidator()
        {
            RuleFor(x => x.Id).RequiredRule("Id");

            RuleFor(x => x.Notes)
                .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.PrescriptionItems)
                .NotEmpty().WithMessage("يجب إضافة دواء واحد على الأقل للروشتة");

            RuleForEach(x => x.PrescriptionItems)
                .SetValidator(new UpdatePrescriptionItemDtoRequestValidator());
        }
    }
}
