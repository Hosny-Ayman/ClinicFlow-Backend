using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.Infrastructure.Data;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _appDbContext;

        public PaymentRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _appDbContext.Payments.AddAsync(payment);
        }
    }
}
