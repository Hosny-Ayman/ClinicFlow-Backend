using ClinicFlow.Application.Features.Authentication.DTOs.Responses;

namespace ClinicFlow.Application.Common.Interfaces
{
    public interface IUserQueryService
    {
        Task<CurrentUserDto?> GetUserProfilByEmaileAsync(string Email);
    }
}
