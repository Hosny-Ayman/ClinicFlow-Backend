using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Application.Features.Doctors.DTOs.Responses;
using ClinicFlow.Application.Features.Users.UserValidators;
using FluentValidation;

namespace ClinicFlow.Application.Features.Doctors.DoctorValidators
{
    public class GetDoctorFullInforamtionDtoResponseValidator:AbstractValidator<UpdateDoctorFullInforamtionDtoRequest>
    {

        public GetDoctorFullInforamtionDtoResponseValidator()
        {
            RuleFor(x => x.User)
           .SetValidator(new UpdateUserInformationDtoRequestValidator());

            RuleFor(x => x.Doctor)
          .SetValidator(new UpdateDoctorInforamtionDtoRequestValidator());
        }

    }
}
