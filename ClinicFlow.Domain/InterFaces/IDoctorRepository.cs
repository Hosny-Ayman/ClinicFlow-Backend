using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.InterFaces
{
    public interface IDoctorRepository
    {

        Task<int> AddDoctorAsync(Doctor doctor);
        Task<Doctor?> GetDoctorByIdAsync(int id,int ClinicId, bool Tracking = false);

    }
}
