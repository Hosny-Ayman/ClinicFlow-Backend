using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class UserBuilder
    {
        public static CreateAndEditUserDtoRequest CreateAndEditUserDto(
            string email = "receptionist@example.com",
            string password = "Password123!",
            string firstName = "Jane",
            string lastName = "Smith",
            string phoneNumber = "0987654321")
        {
            return new CreateAndEditUserDtoRequest
            {
                Email = email,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber
            };
        }

        public static UpdateUserInformationDtoRequest UpdateUserDto(
            int id = 1,
            string email = "updated@example.com",
            string password = "NewPassword123!",
            string firstName = "UpdatedJane",
            string lastName = "UpdatedSmith",
            string phoneNumber = "1112223333",
            bool isActive = true)
        {
            return new UpdateUserInformationDtoRequest
            {
                Id = id,
                Email = email,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                IsActive = isActive
            };
        }

        public static ReceptionistsSearchDtoRequest CreateReceptionistsSearchDtoRequest()
        {
            return new ReceptionistsSearchDtoRequest
            {
                PageNumber = 1,
                PageSize = 10
            };
        }

        public static User CreateUserEntity(
            int id = 1,
            int clinicId = 10,
            string email = "receptionist@example.com",
            string passwordHash = "hashedpassword",
            bool isActive = true)
        {
            return new User
            {
                Id = id,
                ClinicId = clinicId,
                PasswordHash = passwordHash,
                IsActive = isActive,
                Person = new Person
                {
                    Id = id + 100,
                    Email = email,
                    FirstName = "Jane",
                    LastName = "Smith",
                    PhoneNumber = "0987654321",
                    CreatedAt = System.DateTime.UtcNow
                }
            };
        }
    }
}
