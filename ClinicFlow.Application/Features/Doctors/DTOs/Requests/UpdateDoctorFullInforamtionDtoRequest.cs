using ClinicFlow.Application.Features.Users.DTOs.Requests;

namespace ClinicFlow.Application.Features.Doctors.DTOs.Requests
{
    public sealed record UpdateDoctorFullInforamtionDtoRequest
    {
        public UpdateDoctorInforamtionDtoRequest Doctor { get; set; } = null!;
        public UpdateUserInformationDtoRequest User { get; set; } = null!;

    }
}
