using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Application.Features.Users.UserValidators;
using FluentValidation;

namespace ClinicFlow.Application.Features.Doctors.DoctorValidators
{
    public class CreateDoctorWithUserDtoRequestValidator:AbstractValidator<CreateAndEditDoctorWithUserDtoRequest>
    {

        public CreateDoctorWithUserDtoRequestValidator()
        {
            RuleFor(x => x.User)
            .SetValidator(new CreateUserDtoRequestValidator());

            RuleFor(x => x.Doctor)
                .SetValidator(new CreateDoctorDtoRequestValidator());
        }

    }
}
