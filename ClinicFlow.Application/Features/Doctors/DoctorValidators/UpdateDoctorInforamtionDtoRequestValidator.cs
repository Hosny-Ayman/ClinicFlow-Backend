using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using FluentValidation;
using System.Reflection;

namespace ClinicFlow.Application.Features.Doctors.DoctorValidators
{
    public class UpdateDoctorInforamtionDtoRequestValidator:AbstractValidator<UpdateDoctorInforamtionDtoRequest>
    {


        public UpdateDoctorInforamtionDtoRequestValidator()
        {
            RuleFor(x => x.Id).RequiredRule("Id");
            RuleFor(x => x.SpecialtyId).RequiredRule("SpecialtyId");
            RuleFor(x => x.ConsultationFee).RequiredRule("ConsultationFee");
            RuleFor(x => x.Gender).RequiredRule("Gender");
            RuleFor(x => x.ExperienceYears).RequiredRule("ExperienceYears");

            


        }
    }
}
