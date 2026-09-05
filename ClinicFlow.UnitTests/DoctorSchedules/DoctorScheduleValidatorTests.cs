using ClinicFlow.Application.Features.DoctorSchedules.DoctorScheduleValidators;
using ClinicFlow.Application.Features.DoctorSchedules.DTOs.Requests;
using System;
using Xunit;

namespace ClinicFlow.UnitTests.DoctorSchedules
{
    public class DoctorScheduleValidatorTests
    {
        private readonly UpdateAndGetDoctorScheduleDtoRequestValidator _validator;

        public DoctorScheduleValidatorTests()
        {
            _validator = new UpdateAndGetDoctorScheduleDtoRequestValidator();
        }

        [Fact]
        public void Validate_WhenDayOfWeekIsSunday_ShouldBeValid()
        {
            // Arrange
            var model = new UpdateAndGetDoctorScheduleDtoRequest
            {
                DayOfWeek = DayOfWeek.Sunday, // 0 - Previously failed with RequiredRule
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsAvailable = true
            };

            // Act
            var result = _validator.Validate(model);

            // Assert
            Assert.True(result.IsValid);
            Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(model.DayOfWeek));
        }

        [Theory]
        [InlineData(DayOfWeek.Sunday)]
        [InlineData(DayOfWeek.Monday)]
        [InlineData(DayOfWeek.Tuesday)]
        [InlineData(DayOfWeek.Wednesday)]
        [InlineData(DayOfWeek.Thursday)]
        [InlineData(DayOfWeek.Friday)]
        [InlineData(DayOfWeek.Saturday)]
        public void Validate_WhenDayOfWeekIsValidEnum_ShouldBeValid(DayOfWeek dayOfWeek)
        {
            // Arrange
            var model = new UpdateAndGetDoctorScheduleDtoRequest
            {
                DayOfWeek = dayOfWeek,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsAvailable = true
            };

            // Act
            var result = _validator.Validate(model);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_WhenDayOfWeekIsInvalidEnum_ShouldHaveValidationError()
        {
            // Arrange
            var model = new UpdateAndGetDoctorScheduleDtoRequest
            {
                DayOfWeek = (DayOfWeek)99, // Invalid enum value
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsAvailable = true
            };

            // Act
            var result = _validator.Validate(model);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(model.DayOfWeek) && e.ErrorMessage == "DayOfWeek must be a valid day.");
        }
    }
}
