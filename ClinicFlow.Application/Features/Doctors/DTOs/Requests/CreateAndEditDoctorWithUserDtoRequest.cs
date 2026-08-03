using ClinicFlow.Application.Features.Users.DTOs.Requests;

namespace ClinicFlow.Application.Features.Doctors.DTOs.Requests
{
    public sealed record CreateAndEditDoctorWithUserDtoRequest
    {

        public CreateAndEditUserDtoRequest User { get; init; } = null!;

        public CreateAndEditDoctorDtoRequest Doctor { get; init; } = null!;
    }
}
