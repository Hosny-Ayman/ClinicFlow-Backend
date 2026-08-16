using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {

        private readonly AppDbContext _appDbContext;


        public PatientRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<int> AddPatientAsync(Patient patient)
        {
            await _appDbContext.Patients.AddAsync(patient);

            return patient.Id;
        }

        public async Task<Patient?> GetPatientByIdAsync(int id, int clinicId, bool tracking = false)
        {
            var query = _appDbContext.Patients.AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();

            return await query
                .Include(p => p.Person)
                .Include(p => p.ClinicPatients)
                .SingleOrDefaultAsync(p => p.Id == id && p.ClinicPatients.Any(cp => cp.ClinicId == clinicId));
        }

        public async Task<bool> IsPatientInClinicAsync(int patientId, int clinicId)
        {
            return await _appDbContext.ClinicPatients
                .AsNoTracking()
                .AnyAsync(cp => cp.PatientId == patientId && cp.ClinicId == clinicId);
        }
    }
}
