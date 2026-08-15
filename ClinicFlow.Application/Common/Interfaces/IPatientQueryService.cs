using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Features.Patients.DTOs.Requests;
using ClinicFlow.Application.Features.Patients.DTOs.Responses;

namespace ClinicFlow.Application.Common.Interfaces
{
    public interface IPatientQueryService
    {

        Task<PagedResponse<GetAllPatientsDtoResponse>> GetAllPatientsAsync(PatientSearchDtoRequest request, int clinicId);

    }
}
