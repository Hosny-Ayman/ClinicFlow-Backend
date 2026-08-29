using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {

        private readonly AppDbContext _appDbContext;

        public AppointmentRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            await _appDbContext.AddAsync(appointment);
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync(DateOnly appointmentDate, int clinicId, int doctorId)
        {

            return await _appDbContext.Appointments.Where(x => x.AppointmentDate == appointmentDate && x.ClinicId == clinicId && x.DoctorId == doctorId).ToListAsync();

        }

        public async Task<bool> IsAppointmentReservedAsync(DateOnly appointmentDate, TimeOnly startTime, int clinicId, int doctorId)
        {
            return await _appDbContext.Appointments.AnyAsync(a => a.AppointmentDate == appointmentDate &&a.StartTime == startTime &&
                a.ClinicId == clinicId && a.DoctorId == doctorId);
        }
    }
}
