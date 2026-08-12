using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Features.Authentication.DTOs.Responses;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Application.Features.Users.DTOs.Responses;

namespace ClinicFlow.Application.Common.Interfaces
{
    public interface IUserQueryService
    {
        Task<CurrentUserDto?> GetUserProfilByEmaileAsync(string Email);
        Task<PagedResponse<GetAllReceptionistsDtoRequest>> GetAllReceptionistsformationsAsync(ReceptionistsSearchDtoRequest request, int clinicId);
    }
}
