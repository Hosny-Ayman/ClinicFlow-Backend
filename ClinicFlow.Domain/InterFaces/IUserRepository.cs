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

        Task<bool> IsUserExistsByIdAsync(int userId ,int clinicId);

        Task<bool> IsEmailExitsAsync(string email, int clinicId);

        Task<bool> IsPhoneExitsAsync(string phone, int clinicId);

        Task<bool> ToggleUserStatusAsync(int userId, int clinicId);



    }
}
