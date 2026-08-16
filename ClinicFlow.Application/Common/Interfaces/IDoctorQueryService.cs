using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Application.Features.Doctors.DTOs.Responses;

namespace ClinicFlow.Application.Common.Interfaces
{
    public interface IDoctorQueryService
    {
        Task<PagedResponse<GetAllDoctorsInformationsDtoResponse>> GetAllDoctorsInformationsAsync(DoctorSearchDtoRequest request, int clinicId);


    }
}
