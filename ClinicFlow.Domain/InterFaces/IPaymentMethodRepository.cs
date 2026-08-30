using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Domain.Interfaces
{
    public interface IPaymentMethodRepository
    {

        Task<PaymentMethod?> GetPaymentMethodByNameAsync(string name,bool tracking = false);
       
    }
}
