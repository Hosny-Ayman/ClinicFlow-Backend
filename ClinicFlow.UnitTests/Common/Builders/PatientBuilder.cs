using ClinicFlow.Application.Features.Patients.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class PatientBuilder
    {
        public static CreatePatientDtoRequest CreatePatientDto(
            string firstName = "Jane",
            string lastName = "Doe",
            string? email = "jane@example.com",
            string phoneNumber = "0987654321",
            GenderEnum gender = GenderEnum.Female)
        {
            return new CreatePatientDtoRequest
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                DateOfBirth = new DateOnly(1990, 1, 1),
                Gender = gender,
                Notes = "No allergies",
                Address = "123 Main St",
                BloodType = BloodTypeEnum.OPositive,
                NationalId = "123456789",
                EmergencyContactName = "John Doe",
                EmergencyContactPhone = "1234567890"
            };
        }

        public static UpdatePatientDtoRequest UpdatePatientDto(
            int id = 1,
            string firstName = "Jane",
            string lastName = "Doe",
            string? email = "jane@example.com",
            string phoneNumber = "0987654321",
            GenderEnum gender = GenderEnum.Female)
        {
            return new UpdatePatientDtoRequest
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                DateOfBirth = new DateOnly(1990, 1, 1),
                Gender = gender,
                Notes = "No allergies",
                Address = "123 Main St",
                BloodType = BloodTypeEnum.OPositive,
                NationalId = "123456789",
                EmergencyContactName = "John Doe",
                EmergencyContactPhone = "1234567890"
            };
        }

        public static Patient CreatePatientEntity(
            int id = 1,
            int clinicId = 10,
            string firstName = "Jane",
            string lastName = "Doe",
            GenderEnum gender = GenderEnum.Female)
        {
            return new Patient
            {
                Id = id,
                PersonId = 100 + id,
                DateOfBirth = new DateOnly(1990, 1, 1),
                Gender = gender,
                Person = new Person
                {
                    Id = 100 + id,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = $"{firstName.ToLower()}@example.com",
                    PhoneNumber = "0987654321",
                    CreatedAt = DateTime.UtcNow
                },
                ClinicPatients = new List<ClinicPatient>
                {
                    new ClinicPatient { ClinicId = clinicId, PatientId = id }
                },
                CreatedAt = DateTime.UtcNow
            };
        }

        public static PatientSearchDtoRequest CreateSearchRequest()
        {
            return new PatientSearchDtoRequest
            {
                PageNumber = 1,
                PageSize = 10
            };
        }

        public static PatientAppointmentSearchDtoRequest CreateAppointmentSearchRequest(
            string phoneNumber = "0987654321")
        {
            return new PatientAppointmentSearchDtoRequest
            {
                PhoneNumber = phoneNumber
            };
        }
    }
}
