using ClinicFlow.Application.Features.Clinics.DTOs.Requests;
using ClinicFlow.Application.Features.Clinics.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class ClinicBuilder
    {
        public static CreateAndEditClinicDtoRequest CreateAndEditClinicDto(
            string name = "Healthy Life Clinic",
            string phone = "123456789",
            string email = "clinic@health.com",
            string address = "123 Main St",
            string? description = "General Clinic",
            bool includeLogo = false,
            bool isImageDeleted = false)
        {
            return new CreateAndEditClinicDtoRequest
            {
                Name = name,
                Phone = phone,
                Email = email,
                Address = address,
                Description = description,
                LogoUrl = includeLogo ? new Mock<IFormFile>().Object : null,
                IsImageDelted = isImageDeleted
            };
        }

        public static Clinic CreateClinicEntity(
            int id = 1,
            string name = "Healthy Life Clinic",
            string? logoUrl = null)
        {
            return new Clinic
            {
                Id = id,
                Name = name,
                Phone = "123456789",
                Email = "clinic@health.com",
                Address = "123 Main St",
                Description = "General Clinic",
                LogoUrl = logoUrl,
                IsActive = true
            };
        }

        public static CreateClinicResponse CreateClinicResponse(int clinicId = 1, string ownerFullName = "Jane Doe")
        {
            return new CreateClinicResponse
            {
                ClinicId = clinicId,
                ClinicName = "Healthy Life Clinic",
                ClinicPhone = "123456789",
                ClinicEmail = "clinic@health.com",
                ClinicAddress = "123 Main St",
                OwnerFullName = ownerFullName
            };
        }
    }
}
