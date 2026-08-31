using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.MedicalRecords.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.MedicalRecords.MedicalRecordValidators
{
    public class UpdateMedicalRecordDtoRequestValidator : AbstractValidator<UpdateMedicalRecordDtoRequest>
    {
        public UpdateMedicalRecordDtoRequestValidator()
        {
            RuleFor(x => x.Id).RequiredRule("Id");

            RuleFor(x => x.Diagnosis)
                .RequiredRule("Diagnosis")
                .MaximumLength(1000).WithMessage("Diagnosis must not exceed 1000 characters.");

            RuleFor(x => x.TreatmentPlan)
                .RequiredRule("TreatmentPlan")
                .MaximumLength(1000).WithMessage("TreatmentPlan must not exceed 1000 characters.");

            RuleFor(x => x.Symptoms)
                .MaximumLength(2000).WithMessage("Symptoms must not exceed 2000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Symptoms));

            RuleFor(x => x.Notes)
                .MaximumLength(3000).WithMessage("Notes must not exceed 3000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
}
