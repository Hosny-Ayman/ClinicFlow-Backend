using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.Interfaces
{
    public interface IDoctorScheduleRepository
    {
        Task AddDoctorSchedulesAsync(List<DoctorSchedule> doctorSchedules);

        Task<List<DoctorSchedule>> GetAllDoctorSchedulesAsync(int doctotrId,int clinicId,bool tracking = false);


    }
}
