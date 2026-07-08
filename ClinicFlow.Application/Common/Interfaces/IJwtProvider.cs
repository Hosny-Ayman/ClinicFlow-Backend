using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Common.Interfaces
{
    public interface IJwtProvider
    {

        string GenerateToken(User user);

    }
}
