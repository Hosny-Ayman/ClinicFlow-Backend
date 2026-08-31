using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.Interfaces
{
    public interface IMedicalRecordRepository
    {
        Task AddMedicalRecordAsync(MedicalRecord medicalRecord);

        Task<MedicalRecord?> GetMedicalRecordByIdAsync(int id, int clinicId, bool tracking = false);

        Task<MedicalRecord?> GetMedicalRecordByAppointmentIdAsync(int appointmentId, int clinicId, bool tracking = false);

        Task<bool> HasMedicalRecordForAppointmentAsync(int appointmentId);
    }
}
