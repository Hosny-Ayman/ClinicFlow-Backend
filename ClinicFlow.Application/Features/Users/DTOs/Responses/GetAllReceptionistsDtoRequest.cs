namespace ClinicFlow.Application.Features.Users.DTOs.Responses
{
    public sealed record GetAllReceptionistsDtoRequest
    {

        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

    }
}
