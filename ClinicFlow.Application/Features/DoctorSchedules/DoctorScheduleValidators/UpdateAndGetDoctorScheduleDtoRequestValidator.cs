using ClinicFlow.Application.Common.ValidationRules;
using ClinicFlow.Application.Features.DoctorSchedules.DTOs.Requests;
using FluentValidation;

namespace ClinicFlow.Application.Features.DoctorSchedules.DoctorScheduleValidators
{
    public class UpdateAndGetDoctorScheduleDtoRequestValidator:AbstractValidator<UpdateAndGetDoctorScheduleDtoRequest>
    {

        public UpdateAndGetDoctorScheduleDtoRequestValidator()
        {
            RuleFor(x => x.DayOfWeek)
    .IsInEnum()
    .WithMessage("DayOfWeek must be a valid day.");
            RuleFor(x => x.Id).NotNull().When(x => x.Id.HasValue);
            RuleFor(x => x.StartTime).RequiredRule("StartTime");
            RuleFor(x => x.EndTime).RequiredRule("EndTime");
            RuleFor(x => x.EndTime)
     .Must((request, endTime) =>
     {
         if (!request.IsAvailable)
             return true;

         var startTime = request.StartTime;
         var actualEndTime = endTime;
        
         return actualEndTime > startTime;
     }).WithMessage("EndTime must be greater than StartTime.");
        }

    }

    public class UpdateDoctorSchedulesRequestValidator : AbstractValidator<List<UpdateAndGetDoctorScheduleDtoRequest>>
    {
        public UpdateDoctorSchedulesRequestValidator(IValidator<UpdateAndGetDoctorScheduleDtoRequest> validator)
        {
            RuleForEach(x => x).SetValidator(validator);
        }
    }
}
