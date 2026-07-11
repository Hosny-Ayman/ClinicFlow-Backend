using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.InterFaces
{
    public interface IUserRepository
    {

        Task<User?> GetUserByIdAsync(int userId, bool Tracking = false);

        Task<User?> GetUserByPhoneNumberAsync(string PhoneNumber, bool Tracking = false);

        Task<User?> GetUserByEmailAsync(string Email, bool Tracking = false);

        Task<int> AddAsync(User user);


        Task<bool> IsUserExistsByIdAsync(int userId);




    }
}
