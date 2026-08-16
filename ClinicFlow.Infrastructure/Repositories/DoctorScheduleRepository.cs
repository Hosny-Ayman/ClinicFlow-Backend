using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class DoctorScheduleRepository : IDoctorScheduleRepository
    {
        private readonly AppDbContext _appDbContext;

        public DoctorScheduleRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddDoctorSchedulesAsync(List<DoctorSchedule> doctorSchedules)
        {
            await _appDbContext.DoctorSchedules.AddRangeAsync(doctorSchedules);
        }

        public async Task<List<DoctorSchedule>> GetAllDoctorSchedulesAsync(int doctotrId, int clinicId, bool tracking = false)
        {
            var query = _appDbContext.DoctorSchedules.AsQueryable();

            if(!tracking)
            {
                query = query.AsNoTracking();
            }


            return await query.Where(ds => ds.DoctorId == doctotrId && ds.Doctor.ClinicId == clinicId).ToListAsync();
        }
    }
}
