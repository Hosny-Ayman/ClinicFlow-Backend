using ClinicFlow.Application.Features.Users.DTOs.Requests;

namespace ClinicFlow.Application.Features.Clinics.DTOs.Requests
{
    public sealed record CreateClinicWithOwnerDtoRequest
    {

        public CreateClinicDtoRequest Clinic { get; set; } = null!;

        public CreateUserDtoRequest User { get; set; } = null!;

    }
}
