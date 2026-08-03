using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.InterFaces
{
    public interface IClinicSetupRepository
    {

        Task<int> AddClinicSetupStatusAsync(ClinicSetup clinicSetup);

        Task UpdateClinicSetupStatusAsync(ClinicSetup clinicSetup);


    }
}
