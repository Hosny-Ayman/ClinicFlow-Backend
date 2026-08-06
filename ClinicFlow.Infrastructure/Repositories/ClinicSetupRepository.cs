using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class ClinicSetupRepository : IClinicSetupRepository
    {

        private readonly AppDbContext _appDbContext;

        public ClinicSetupRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<int> AddClinicSetupStatusAsync(ClinicSetup clinicSetup)
        {
            await _appDbContext.AddAsync(clinicSetup);

            return clinicSetup.Id;
        }

        public async Task<ClinicSetup?> GetClinicSetupAsync(int clinicId, bool treacking = false)
        {
            var query = _appDbContext.ClinicSetups.AsQueryable();

            if (!treacking)
                query = query.AsNoTracking();

            return await query.SingleOrDefaultAsync(x => x.ClinicId == clinicId);
        }

        public async Task<bool> IsClinicSetupExistsAsync(int clinicId)
        {
            return await _appDbContext.ClinicSetups.AnyAsync(x => x.ClinicId == clinicId);
        }

        public async Task UpdateClinicSetupStatusAsync(ClinicSetup clinicSetup)
        {
             _appDbContext.ClinicSetups.Update(clinicSetup);
          
        }
    }
}
