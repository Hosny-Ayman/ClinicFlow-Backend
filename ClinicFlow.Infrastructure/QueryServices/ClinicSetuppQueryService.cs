using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Features.ClinicSetups.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ClinicFlow.Infrastructure.QueryServices
{
    public class ClinicSetuppQueryService: IClinicSetupQueryService
    {
        private readonly AppDbContext _appDbContext;

        public ClinicSetuppQueryService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<GetClinicSetupStatusDtoResponse?> GetClinicSetupStatusAsync(int ClinicId, bool Tracking = false)
        {
            var query = _appDbContext.ClinicSetups.AsQueryable();


            if (!Tracking)
                query = query.AsNoTracking();


            var hasDoctor = query.Any(x =>x.clinic.Id == ClinicId && x.clinic.Doctors.Any());

            var hasReceptionist = query.Where(x => x.clinic.Id == ClinicId).Any(x => x.clinic.Users.Any(u => u.UserRoles.Any(ur => ur.RoleId == (int)RoleEnum.Receptionist)));

            var hasWorkingHours = query.Where(x => x.clinic.Id == ClinicId).Any(x => x.clinic.ClinicWorkingHours.Any());


            int completedSteps = 0;

            if (hasDoctor)
                completedSteps++;

            if (hasReceptionist)
                completedSteps++;

            if (hasWorkingHours)
                completedSteps++;

            double progress = (completedSteps / 3.0) * 100;


            var ClinicSetupStatus = new GetClinicSetupStatusDtoResponse
            {
                IsSetupCompleted = hasDoctor && hasReceptionist && hasWorkingHours,
                HasSkippedSetup = await _appDbContext.ClinicSetups.Where(x => x.ClinicId == ClinicId).Select(x => x.HasSkippedSetup).FirstOrDefaultAsync(),
                Progress = progress,

                Steps = new List<SetupStepDtoRequest>
                {
                    new SetupStepDtoRequest
                    {
                        Key = "doctor",
                        Title = "إضافة أول طبيب",
                        IsCompleted = hasDoctor
                    },

                    new SetupStepDtoRequest
                    {
                        Key = "receptionist",
                        Title = "إضافة أول موظف استقبال",
                        IsCompleted = hasReceptionist
                    },

                     new SetupStepDtoRequest
                    {
                        Key = "workingHours",
                        Title = "تحديد مواعيد العمل",
                        IsCompleted = hasWorkingHours
                    },
                }


            };


            return ClinicSetupStatus;
        }

    }
}
