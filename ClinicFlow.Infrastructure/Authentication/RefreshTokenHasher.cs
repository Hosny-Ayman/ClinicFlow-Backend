using ClinicFlow.Application.Common.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace ClinicFlow.Infrastructure.Authentication
{
    public class RefreshTokenHasher : IRefreshTokenHasher
    {
        public string Hash(string refreshToken)
        {
            var bytes = Encoding.UTF8.GetBytes(refreshToken);

            var hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }
    }
}
