using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.Interfaces
{
    public interface IAppointmentRepository
    {

        Task AddAppointmentAsync(Appointment appointment);

        Task<bool> IsAppointmentReservedAsync(DateOnly appointmentDate, TimeOnly startTime ,int clinicId,int doctorId);

        Task<List<Appointment>> GetAllAppointmentsAsync(DateOnly appointmentDate, int clinicId, int doctorId);
    }
}
