using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.InterFaces
{
    public interface IUserRepository
    {

        Task<User?> GetUserByIdAsync(int userId,int clinicId, bool Tracking = false);

        Task<User?> GetUserByPhoneNumberAsync(string PhoneNumber, int clinicId, bool Tracking = false);

        Task<User?> GetUserByEmailAsync(string Email, bool Tracking = false);

        Task<User?> GetUserByDoctorIdAsync(int DoctorId, int clinicId, bool Tracking = false);

        Task<int> AddAsync(User user);

        Task<bool> IsUserExistsByIdAsync(int userId);

        Task<bool> IsEmailExitsAsync(string email);

        Task<bool> IsPhoneExitsAsync(string phone);



    }
}
