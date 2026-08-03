using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly AppDbContext _appDbContext;

        public SpecialtyRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Specialty>> getAllSpecialtiesAsync()
        {
           return await _appDbContext.Specialties.ToListAsync();
        }

        public async Task<Specialty?> getSpecialtyById(int SpecialtyId)
        {
            return await _appDbContext.Specialties.SingleOrDefaultAsync(x => x.Id == SpecialtyId);
        }
    }
}
