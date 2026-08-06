using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Features.ClinicSetups.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ClinicFlow.Infrastructure.QueryServices
{
    public class ClinicSetupQueryService: IClinicSetupQueryService
    {
        private readonly AppDbContext _appDbContext;

        public ClinicSetupQueryService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<GetClinicSetupStatusDtoResponse?> GetClinicSetupStatusAsync(int ClinicId, bool Tracking = false)
        {
            var hasDoctor = await _appDbContext.Doctors.AnyAsync(x => x.ClinicId == ClinicId);

            var hasReceptionist = await _appDbContext.Users.AnyAsync(x => x.ClinicId == ClinicId &&
            x.UserRoles.Any(ur => ur.RoleId == (int)RoleEnum.Receptionist));

            var hasWorkingHours = await _appDbContext.ClinicWorkingHours.AnyAsync(x => x.ClinicId == ClinicId);

            var hasSkippedSetup = await _appDbContext.ClinicSetups
                .Where(x => x.ClinicId == ClinicId)
                .Select(x => x.HasSkippedSetup)
                .FirstOrDefaultAsync();

            int completedSteps = 0;
            if (hasDoctor) completedSteps++;
            if (hasReceptionist) completedSteps++;
            if (hasWorkingHours) completedSteps++;

            double progress = (completedSteps / 3.0) * 100;

            return new GetClinicSetupStatusDtoResponse
            {
                IsSetupCompleted = hasDoctor && hasReceptionist && hasWorkingHours,
                HasSkippedSetup = hasSkippedSetup,
                Progress = progress,
                Steps = new List<SetupStepDtoRequest>
                {
                     new SetupStepDtoRequest { Key = "doctor", Title = "إضافة أول طبيب", IsCompleted = hasDoctor },
                     new SetupStepDtoRequest { Key = "receptionist", Title = "إضافة موظف استقبال", IsCompleted = hasReceptionist },
                     new SetupStepDtoRequest { Key = "workingHours", Title = "تحديد مواعيد العمل", IsCompleted = hasWorkingHours }
                }
            };  
        }

    }
}
