using ClinicFlow.Application.Common.Interfaces;
using System.Security.Cryptography;

namespace ClinicFlow.Infrastructure.Authentication
{
    public class RefreshTokenGenerator: IRefreshTokenGenerator
    {

        public string Generate()
        {
            var bytes = new byte[64];

            RandomNumberGenerator.Fill(bytes);

            return Convert.ToBase64String(bytes);
        }

    }
}
