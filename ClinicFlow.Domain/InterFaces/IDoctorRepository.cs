using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.Interfaces
{
    public interface IDoctorRepository
    {

        Task<int> AddDoctorAsync(Doctor doctor);
        Task<Doctor?> GetDoctorByIdAsync(int id,int ClinicId, bool Tracking = false);
        Task<bool> IsDoctorBelongToClinic(int doctorId, int clinicId);
        Task<int?> GetDoctorIdByUserId(int UserId, int ClinicId);
        Task<Doctor?> GetDoctorByUserIdAsync(int UserId, int ClinicId, bool Tracking = false);


    }
}
