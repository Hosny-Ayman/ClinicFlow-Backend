using ClinicFlow.Application.Features.Authentication.DTOs.Requests;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class AuthenticationBuilder
    {

        public static LoginDtoRequest Create()
        {
            return new LoginDtoRequest
            {
                Email = "Hosny@gmail.com",
                Password = "Hosny@123SSEDD"
            };
        }

    }
}
