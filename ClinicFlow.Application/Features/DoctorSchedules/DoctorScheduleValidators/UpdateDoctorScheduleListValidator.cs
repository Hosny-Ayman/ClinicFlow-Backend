using ClinicFlow.Application.Features.DoctorSchedules.DTOs.Requests;
using FluentValidation;
using System.Collections.Generic;

namespace ClinicFlow.Application.Features.DoctorSchedules.DoctorScheduleValidators
{
    public class UpdateDoctorScheduleListValidator : AbstractValidator<List<UpdateAndGetDoctorScheduleDtoRequest>>
    {
        public UpdateDoctorScheduleListValidator()
        {
            RuleForEach(x => x).SetValidator(new UpdateAndGetDoctorScheduleDtoRequestValidator());
        }
    }
}
