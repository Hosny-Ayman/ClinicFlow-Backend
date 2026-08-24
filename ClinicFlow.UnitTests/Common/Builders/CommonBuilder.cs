using ClinicFlow.Domain.Entities;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class CommonBuilder
    {

        public static User CreateUser()
        {
            return new User
            {
                Id = 1,
                PersonId = 1,
                ClinicId = 1,
                PasswordHash = "H##52fsgGG22",
                IsActive = true,
                CreatedAt = DateTime.Now,
            };
        }

        public static string CreateRefreshToken()
        {
            return "esassd2324@#$%$asf5353";
        }

        public static string CreateAccessToken()
        {
            return "AccessToken$##%##gfdgd22";
        }

        public static string CreateHash()
        {
            return "ddddddddddddddddddddddd";
        }
    }
}
