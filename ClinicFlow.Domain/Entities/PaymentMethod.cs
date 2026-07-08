namespace ClinicFlow.Domain.Entities
{
    public class PaymentMethod
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }




















}
