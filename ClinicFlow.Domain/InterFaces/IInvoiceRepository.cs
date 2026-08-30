using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.Interfaces
{
    public interface IInvoiceRepository
    {

        Task AddInvoiceAsync(Invoice invoice);
    
    }
}
