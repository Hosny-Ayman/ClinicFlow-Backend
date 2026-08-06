using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.InterFaces
{
    public interface IClinicSetupRepository
    {

        Task<int> AddClinicSetupStatusAsync(ClinicSetup clinicSetup);

        Task UpdateClinicSetupStatusAsync(ClinicSetup clinicSetup);

        Task<ClinicSetup?> GetClinicSetupAsync(int clinicId,bool treacking = false);

        Task<bool> IsClinicSetupExistsAsync(int clinicId);


    }
}
