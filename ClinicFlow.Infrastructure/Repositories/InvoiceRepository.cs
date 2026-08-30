using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.Infrastructure.Data;

namespace ClinicFlow.Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {

        private readonly AppDbContext _appDbContext;

        public InvoiceRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddInvoiceAsync(Invoice invoice)
        {
            await _appDbContext.Invoices.AddAsync(invoice);
           
        }

    }
}
