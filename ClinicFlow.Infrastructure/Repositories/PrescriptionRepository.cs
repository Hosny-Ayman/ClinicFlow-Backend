using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly AppDbContext _appDbContext;

        public PrescriptionRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddPrescriptionAsync(Prescription prescription)
        {
            await _appDbContext.Prescriptions.AddAsync(prescription);
        }

        public async Task<Prescription?> GetPrescriptionByIdAsync(int id, int clinicId, bool tracking = false)
        {
            var query = _appDbContext.Prescriptions.AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();

            return await query
                .Include(p => p.PrescriptionItems)
                .SingleOrDefaultAsync(p => p.Id == id && p.MedicalRecord.Appointment.ClinicId == clinicId);
        }

        public async Task<Prescription?> GetPrescriptionByMedicalRecordIdAsync(int medicalRecordId, int clinicId, bool tracking = false)
        {
            var query = _appDbContext.Prescriptions.AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();

            return await query
                .Include(p => p.PrescriptionItems)
                .SingleOrDefaultAsync(p => p.MedicalRecordId == medicalRecordId && p.MedicalRecord.Appointment.ClinicId == clinicId);
        }

        public async Task<bool> HasPrescriptionForMedicalRecordAsync(int medicalRecordId)
        {
            return await _appDbContext.Prescriptions.AnyAsync(p => p.MedicalRecordId == medicalRecordId);
        }
    }
}
