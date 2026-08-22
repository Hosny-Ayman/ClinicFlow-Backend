using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.Interfaces
{
    public interface IDoctorVacationRepository
    {

        Task AddDoctorVacationAsync(DoctorVacation doctorVacation);

        Task<DoctorVacation?> GetDoctorVacationByIdAsync(int id,int doctorId, int clinicId,bool tracking=false);
      
    }
}
