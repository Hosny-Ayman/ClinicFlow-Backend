using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly AppDbContext _appDbContext;

        public MedicalRecordRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddMedicalRecordAsync(MedicalRecord medicalRecord)
        {
            await _appDbContext.MedicalRecords.AddAsync(medicalRecord);
        }

        public async Task<MedicalRecord?> GetMedicalRecordByIdAsync(int id, int clinicId, bool tracking = false)
        {
            var query = _appDbContext.MedicalRecords.AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();

            return await query.SingleOrDefaultAsync(m => m.Id == id && m.Appointment.ClinicId == clinicId);
        }

        public async Task<MedicalRecord?> GetMedicalRecordByAppointmentIdAsync(int appointmentId, int clinicId, bool tracking = false)
        {
            var query = _appDbContext.MedicalRecords.AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();

            return await query.SingleOrDefaultAsync(m => m.AppointmentId == appointmentId && m.Appointment.ClinicId == clinicId);
        }

        public async Task<bool> HasMedicalRecordForAppointmentAsync(int appointmentId)
        {
            return await _appDbContext.MedicalRecords.AnyAsync(m => m.AppointmentId == appointmentId);
        }
    }
}
