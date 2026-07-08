using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class UserReposittory : IUserRepository
    {

        private readonly AppDbContext _appDbContext;


        public UserReposittory(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public async Task<int> AddAsync(User user)
        {
            await _appDbContext.AddAsync(user);

            await _appDbContext.SaveChangesAsync();

            return user.Id;
        }

        public async Task<User?> GetUserByIdAsync(int userId, bool Tracking = false)
        {
            var query = _appDbContext.Users.AsQueryable();

            if (!Tracking)
                query = query.AsNoTracking();


            return await query.SingleOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<User?> GetUserByPhoneNumberAsync(string PhoneNumber, bool Tracking = false)
        {
            var query = _appDbContext.Users.AsQueryable();

            if (!Tracking)
                query = query.AsNoTracking();


            return await query.SingleOrDefaultAsync(x => x.PhoneNumber == PhoneNumber);
        }

        public async Task<User?> GetUserByEmailAsync(string Email, bool Tracking = false)
        {
            var query = _appDbContext.Users.AsQueryable();

            if (!Tracking)
                query = query.AsNoTracking();


            return await query.SingleOrDefaultAsync(x => x.Email == Email);
        }

        public async Task<bool> IsUserExistsByIdAsync(int userId)
        {
            return await _appDbContext.Users.AnyAsync(x => x.Id == userId);
        }

        public async Task UpdateAsync()
        {
            await _appDbContext.SaveChangesAsync();
        }
    }
}
