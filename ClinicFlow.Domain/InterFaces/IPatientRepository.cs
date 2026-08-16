using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.Interfaces
{
    public interface IPatientRepository
    {

        Task<int> AddPatientAsync(Patient patient);
        Task<Patient?> GetPatientByIdAsync(int id, int clinicId, bool tracking = false);
        Task<bool> IsPatientInClinicAsync(int patientId, int clinicId);

    }
}
