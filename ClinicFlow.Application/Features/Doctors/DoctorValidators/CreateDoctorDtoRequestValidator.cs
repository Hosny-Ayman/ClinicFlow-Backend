using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using FluentValidation;
using ClinicFlow.Application.Common.ValidationRules;
namespace ClinicFlow.Application.Features.Doctors.DoctorValidators
{
    public class CreateDoctorDtoRequestValidator:AbstractValidator<CreateDoctorDtoRequest>
    {


        public CreateDoctorDtoRequestValidator()
        {
            RuleFor(x => x.UserId).RequiredRule("UserId");
            RuleFor(x => x.ClinicId).RequiredRule("ClinicId");
            RuleFor(x => x.SpecialtyId).RequiredRule("SpecialtyId");
            RuleFor(x => x.ConsultationFee).RequiredRule("ConsultationFee");
            RuleFor(x => x.Gender).RequiredRule("Gender");
            RuleFor(x => x.ExperienceYears).RequiredRule("ExperienceYears");
           


        }

    }
}
