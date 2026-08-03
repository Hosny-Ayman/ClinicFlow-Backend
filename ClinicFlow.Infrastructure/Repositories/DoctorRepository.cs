using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {

        private readonly AppDbContext _appDbContext;


        public DoctorRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<int> AddDoctorAsync(Doctor doctor)
        {
           await _appDbContext.Doctors.AddAsync(doctor);

            return doctor.Id;
        }

        public async Task<Doctor?> GetDoctorByIdAsync(int id, int ClinicId, bool Tracking = false)
        {
            var query = _appDbContext.Doctors.AsQueryable();

            if (!Tracking)
                query = query.AsNoTracking();


            return await query.Include(x=>x.Specialty).SingleOrDefaultAsync(x => x.Id == id && x.ClinicId == ClinicId);
        }
    }
}
