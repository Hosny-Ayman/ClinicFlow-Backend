namespace ClinicFlow.Application.Features.Clinics.DTOs.Requests
{
    public sealed record UpdateClinicDtoRequest
    {

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? LogoUrl { get; set; }

        public string Phone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; } 


    }
}
