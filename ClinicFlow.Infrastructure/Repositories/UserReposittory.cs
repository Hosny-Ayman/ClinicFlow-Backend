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


            return user.Id;
        }

        public async Task<User?> GetUserByIdAsync(int userId, int clinicId, bool Tracking = false)
        {
            var query = _appDbContext.Users.AsQueryable();

            if (!Tracking)
                query = query.AsNoTracking();


            return await query.SingleOrDefaultAsync(x => x.Id == userId && x.ClinicId == clinicId);
        }

        public async Task<User?> GetUserByPhoneNumberAsync(string PhoneNumber, int clinicId, bool Tracking = false)
        {
            var query = _appDbContext.Users.AsQueryable();

            if (!Tracking)
                query = query.AsNoTracking();


            return await query.SingleOrDefaultAsync(x => x.PhoneNumber == PhoneNumber && x.ClinicId == clinicId);
        }

        public async Task<User?> GetUserByEmailAsync(string Email, bool Tracking = false)
        {
            var query = _appDbContext.Users.AsQueryable();

            if (!Tracking)
                query = query.AsNoTracking();


            return await query.Include(x=>x.UserRoles).ThenInclude(x=>x.Role).SingleOrDefaultAsync(x => x.Email == Email );
        }

        public async Task<bool> IsUserExistsByIdAsync(int userId)
        {
            return await _appDbContext.Users.AnyAsync(x => x.Id == userId);
        }

        public async Task<bool> IsEmailExitsAsync(string email)
        {
            return await _appDbContext.Users.AnyAsync(x => x.Email == email);
        }

        public async Task<bool> IsPhoneExitsAsync(string phone)
        {
            return await _appDbContext.Users.AnyAsync(x => x.PhoneNumber == phone);
        }

        public async Task<User?> GetUserByDoctorIdAsync(int DoctorId, int clinicId, bool Tracking = false)
        {
            var query = _appDbContext.Users.AsQueryable();

            if (!Tracking)
                query = query.AsNoTracking();


            return await query.SingleOrDefaultAsync(x => x.Doctor!.Id == DoctorId && x.ClinicId == clinicId);
        }

      
    }
}
