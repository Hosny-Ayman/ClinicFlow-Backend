using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class PaymentMethodRepository: IPaymentMethodRepository
    {
        private readonly AppDbContext _appDbContext;

        public PaymentMethodRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<PaymentMethod?> GetPaymentMethodByNameAsync(string name, bool tracking = false)
        {
            var query = _appDbContext.PaymentMethods.AsQueryable();

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            return await query.SingleOrDefaultAsync(pm => pm.Name == name);
        }
    }
}
