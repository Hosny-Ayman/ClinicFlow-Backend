using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class DoctorBuilder
    {
        public static CreateAndEditDoctorDtoRequest CreateDoctorDto(
            int specialtyId = 1,
            decimal consultationFee = 100m,
            string bio = "Experienced doctor",
            GenderEnum gender = GenderEnum.Male,
            int experienceYears = 5,
            string? profileImageFileName = null)
        {
            return new CreateAndEditDoctorDtoRequest
            {
                SpecialtyId = specialtyId,
                ConsultationFee = consultationFee,
                Bio = bio,
                Gender = gender,
                ExperienceYears = experienceYears,
                ProfileImage = profileImageFileName != null ? new Moq.Mock<IFormFile>().Object : null
            };
        }

        public static UpdateDoctorInforamtionDtoRequest UpdateDoctorDto(
            int id = 1,
            Microsoft.AspNetCore.Http.IFormFile? newProfileImageUrl = null,
            bool isImageDeleted = false)
        {
            return new UpdateDoctorInforamtionDtoRequest
            {
                Id = id,
                ProfileImageUrl = newProfileImageUrl,
                IsImageDeleted = isImageDeleted
            };
        }

        public static CreateAndEditUserDtoRequest CreateUserDto(
            string email = "test@example.com",
            string password = "Password123!",
            string firstName = "John",
            string lastName = "Doe",
            string phoneNumber = "1234567890")
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
            string firstName = "John",
            string lastName = "Doe",
            string phoneNumber = "1234567890")
        {
            return new UpdateUserInformationDtoRequest
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber
            };
        }

        public static DoctorSearchDtoRequest CreateSearchRequest(
            string? searchTerm = null,
            int? specialtyId = null,
            GenderEnum? gender = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return new DoctorSearchDtoRequest
            {
                FullNameSearch = searchTerm,
                SpecialtyId = specialtyId,
                Gender = gender,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public static Doctor CreateDoctorEntity(
            int id = 1,
            int userId = 1,
            int clinicId = 10,
            string? profileImageUrl = null,
            Specialty? specialty = null)
        {
            return new Doctor
            {
                Id = id,
                UserId = userId,
                ClinicId = clinicId,
                ProfileImageUrl = profileImageUrl,
                Specialty = specialty ?? new Specialty { Id = 1, Name = "Cardiology" }
            };
        }

        public static User CreateUserEntity(
            int id = 1,
            string email = "test@example.com",
            string firstName = "John",
            string lastName = "Doe",
            string phoneNumber = "1234567890",
            bool isActive = true)
        {
            return new User
            {
                Id = id,
                Person = new Person { Email = email, FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber },
                IsActive = isActive
            };
        }
    }
}
