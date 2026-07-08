using  ClinicFlow.Domain.Enums;

namespace ClinicFlow.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public int PaymentMethodId { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatusEnum Status { get; set; }

        public string? TransactionId { get; set; }

        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        public Invoice Invoice { get; set; } = null!;

        public PaymentMethod PaymentMethod { get; set; } = null!;
    }
























}
