namespace ClinicFlow.Application.Features.Prescriptions.DTOs.Requests
{
    public sealed record UpdatePrescriptionDtoRequest
    {
        public int Id { get; init; }
        public string? Notes { get; init; }
        public List<UpdatePrescriptionItemDtoRequest> PrescriptionItems { get; init; } = new();
    }
}
