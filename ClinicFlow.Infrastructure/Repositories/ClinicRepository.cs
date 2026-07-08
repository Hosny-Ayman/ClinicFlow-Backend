using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly AppDbContext _appDbContext;
            
        public ClinicRepository(AppDbContext appDbContext) 
        {
            _appDbContext = appDbContext;


        }


        public async Task<int> AddAsync(Clinic clinic,User user)
        {

            await _appDbContext.Clinics.AddAsync(clinic);

            user.Clinic = clinic;

            await _appDbContext.Users.AddAsync(user);

            await _appDbContext.SaveChangesAsync();

            return clinic.Id;
        }

        public async Task<Clinic?> GetClinicByIdAsync(int clinicId, bool Tracking = false)
        {
            var query = _appDbContext.Clinics.AsQueryable();

            if (!Tracking)
                query = query.AsNoTracking();


            return await query.SingleOrDefaultAsync(x => x.Id == clinicId);
        }

        public async Task<Clinic?> GetClinicByNameAsync(string clinicName, bool Tracking = false)
        {
            var query = _appDbContext.Clinics.AsQueryable();

            if (!Tracking)
                query = query.AsNoTracking();


            return await query.SingleOrDefaultAsync(x => x.Name == clinicName);
        }

        public async Task<bool> IsClinicExistsByIdAsync(int clinicId)
        {
            return await _appDbContext.Clinics.AnyAsync(x => x.Id == clinicId);
        }

        public async Task UpdateAsync()
        {
                await _appDbContext.SaveChangesAsync() ;

        }
    }
}
