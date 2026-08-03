using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using FluentValidation;
using ClinicFlow.Application.Common.ValidationRules;
namespace ClinicFlow.Application.Features.Doctors.DoctorValidators
{
    public class CreateDoctorDtoRequestValidator:AbstractValidator<CreateAndEditDoctorDtoRequest>
    {


        public CreateDoctorDtoRequestValidator()
        {
          
            RuleFor(x => x.SpecialtyId).RequiredRule("SpecialtyId");
            RuleFor(x => x.ConsultationFee).RequiredRule("ConsultationFee");
            RuleFor(x => x.Gender).RequiredRule("Gender");
            RuleFor(x => x.ExperienceYears).RequiredRule("ExperienceYears");
           


        }

    }
}
