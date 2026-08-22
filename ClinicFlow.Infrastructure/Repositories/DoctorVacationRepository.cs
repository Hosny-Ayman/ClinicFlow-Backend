using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class DoctorVacationRepository : IDoctorVacationRepository
    {
        private AppDbContext _appDbContext;

        public DoctorVacationRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddDoctorVacationAsync(DoctorVacation doctorVacation)
        {
            await _appDbContext.DoctorVacations.AddAsync(doctorVacation);
        }

        public async Task<DoctorVacation?> GetDoctorVacationByIdAsync(int id, int doctorId, int clinicId, bool tracking = false)
        {
            var query = _appDbContext.DoctorVacations.AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(x => x.Id == id && x.DoctorId == doctorId && x.Doctor.ClinicId == clinicId);
        }
    }
}
