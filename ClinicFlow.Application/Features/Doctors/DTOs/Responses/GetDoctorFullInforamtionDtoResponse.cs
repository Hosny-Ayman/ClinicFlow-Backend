using ClinicFlow.Application.Features.Users.DTOs.Responses;

namespace ClinicFlow.Application.Features.Doctors.DTOs.Responses
{
    public sealed record GetDoctorFullInforamtionDtoResponse
    {

       public GetUserInformationDtoResponse User { get; init; } = null!;
       public GetDoctorInforamtionDtoResponse Doctor { get; init; } = null!;
    }
}
