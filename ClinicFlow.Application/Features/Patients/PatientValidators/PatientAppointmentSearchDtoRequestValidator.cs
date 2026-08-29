using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.Patients.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.Patients.PatientValidators
{
    public  class PatientAppointmentSearchDtoRequestValidator :AbstractValidator<PatientAppointmentSearchDtoRequest>
    {

        public PatientAppointmentSearchDtoRequestValidator()
        {
            RuleFor(x => x.Name).FirtsNameRule();
            RuleFor(x => x.PhoneNumber).PhoneRule();
        }
    }
}
