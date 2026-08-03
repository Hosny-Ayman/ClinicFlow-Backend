using ClinicFlow.Application.Features.ClinicSetups.DTOs.Responses;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Common.Interfaces
{
    public interface IClinicSetupQueryService
    {

        Task<GetClinicSetupStatusDtoResponse?> GetClinicSetupStatusAsync(int ClinicId, bool Tracking = false);


    }
}
