using ClinicFlow.Domain.Entities;
using System.ComponentModel;

namespace ClinicFlow.Domain.InterFaces
{
    public interface IClinicWorkingHourRepository
    {

        Task AddWorkingHoursAndDaysAsync(List<ClinicWorkingHour> Days);

        Task<List<ClinicWorkingHour>> GetAllWorkingHoursAndDaysAsync(int clinicId,bool tracking = false);

    }
}
