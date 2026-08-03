namespace ClinicFlow.Application.Features.Specialties.DTOs.Requests
{
    public sealed record GetAllSpecialityDtoRequest
    {
        public int Id { get; init; }

        public string Name { get; init; } = null!;

        public bool IsActive { get; init; } 


    }
}
