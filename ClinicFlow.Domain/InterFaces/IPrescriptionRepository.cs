using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task AddPrescriptionAsync(Prescription prescription);

        Task<Prescription?> GetPrescriptionByIdAsync(int id, int clinicId, bool tracking = false);

        Task<Prescription?> GetPrescriptionByMedicalRecordIdAsync(int medicalRecordId, int clinicId, bool tracking = false);

        Task<bool> HasPrescriptionForMedicalRecordAsync(int medicalRecordId);
    }
}
