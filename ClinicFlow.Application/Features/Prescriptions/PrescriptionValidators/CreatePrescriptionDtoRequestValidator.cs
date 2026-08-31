using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Prescriptions.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.Prescriptions.PrescriptionValidators
{
    public class CreatePrescriptionDtoRequestValidator : AbstractValidator<CreatePrescriptionDtoRequest>
    {
        public CreatePrescriptionDtoRequestValidator()
        {
            RuleFor(x => x.MedicalRecordId).RequiredRule("MedicalRecordId");

            RuleFor(x => x.Notes)
                .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.PrescriptionItems)
                .NotEmpty().WithMessage("يجب إضافة دواء واحد على الأقل للروشتة");

            RuleForEach(x => x.PrescriptionItems)
                .SetValidator(new CreatePrescriptionItemDtoRequestValidator());
        }
    }
}
