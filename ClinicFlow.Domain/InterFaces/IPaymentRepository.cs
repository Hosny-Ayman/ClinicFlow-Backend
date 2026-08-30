using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.Interfaces
{
    public interface IPaymentRepository
    {

        Task AddPaymentAsync(Payment payment);

    }
}
