using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.InterFaces
{
    public interface IRefreshTokenRepository
    {

        Task AddAsync(RefreshToken refreshToken);

        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash,bool Tracking = false);

    }
}
