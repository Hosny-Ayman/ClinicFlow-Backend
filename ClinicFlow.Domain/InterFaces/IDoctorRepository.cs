using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.InterFaces
{
    public interface IDoctorRepository
    {

        Task<int> AddDoctorAsync(Doctor doctor);

    }
}
