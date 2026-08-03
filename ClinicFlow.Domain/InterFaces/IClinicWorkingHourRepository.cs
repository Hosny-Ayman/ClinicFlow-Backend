using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.InterFaces
{
    public interface IClinicWorkingHourRepository
    {

        Task AddWorkingHoursAndDaysAsync(List<ClinicWorkingHour> Days);

    }
}
