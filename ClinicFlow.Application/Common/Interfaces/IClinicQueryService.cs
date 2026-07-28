using ClinicFlow.Application.Features.Clinics.DTOs.Responses;

namespace ClinicFlow.Application.Common.Interfaces
{
    public interface  IClinicQueryService
    {

        Task<CreateClinicResponse?> GetClinicInfoWithOwnerFullnameAsync(int ClinicId);


    }
}
