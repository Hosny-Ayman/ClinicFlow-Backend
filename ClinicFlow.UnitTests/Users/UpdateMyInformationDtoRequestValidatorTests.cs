using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Application.Features.Users.UserValidators;
using Xunit;

namespace ClinicFlow.UnitTests.Users
{
    public class UpdateMyInformationDtoRequestValidatorTests
    {
        private readonly UpdateMyInformationDtoRequestValidator _validator;

        public UpdateMyInformationDtoRequestValidatorTests()
        {
            _validator = new UpdateMyInformationDtoRequestValidator();
        }

        [Fact]
        public void Validate_WhenAllFieldsAreValid_ShouldPassValidation()
        {
            // Arrange
            var request = new UpdateMyInformationDtoRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "01234567890"
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t\n")]
        public void Validate_WhenFirstNameIsEmptyOrWhitespace_ShouldFail(string? firstName)
        {
            // Arrange
            var request = new UpdateMyInformationDtoRequest
            {
                FirstName = firstName!,
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "01234567890"
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(request.FirstName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t\n")]
        public void Validate_WhenLastNameIsEmptyOrWhitespace_ShouldFail(string? lastName)
        {
            // Arrange
            var request = new UpdateMyInformationDtoRequest
            {
                FirstName = "John",
                LastName = lastName!,
                Email = "john.doe@example.com",
                PhoneNumber = "01234567890"
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(request.LastName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("not-an-email")]
        [InlineData("@missingusername.com")]
        public void Validate_WhenEmailIsInvalidOrWhitespace_ShouldFail(string? email)
        {
            // Arrange
            var request = new UpdateMyInformationDtoRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = email!,
                PhoneNumber = "01234567890"
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(request.Email));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("12345")] // too short (< 11)
        [InlineData("123456789012345678901")] // too long (> 20)
        [InlineData("0123456789a")] // contains non-digits
        public void Validate_WhenPhoneNumberIsInvalidOrWhitespace_ShouldFail(string? phone)
        {
            // Arrange
            var request = new UpdateMyInformationDtoRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = phone!
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(request.PhoneNumber));
        }
    }
}
