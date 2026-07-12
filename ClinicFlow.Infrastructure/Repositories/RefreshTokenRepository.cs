using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _appDbContext;
        public RefreshTokenRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public async Task AddAsync(RefreshToken refreshToken)
        {
           await _appDbContext.RefreshTokens.AddAsync(refreshToken);

        }

        public async Task<List<RefreshToken>> GetAllActiveTokensByUserIdAsync(int userId, bool tracking = false)
        {
            var query = _appDbContext.RefreshTokens.AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, bool tracking = false)
        {
            var query = _appDbContext.RefreshTokens.AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();

            return await query.Include(x=>x.User).ThenInclude(x=>x.UserRoles).ThenInclude(x=>x.Role).SingleOrDefaultAsync(x => x.TokenHash == tokenHash);
        }

       
    }
}
