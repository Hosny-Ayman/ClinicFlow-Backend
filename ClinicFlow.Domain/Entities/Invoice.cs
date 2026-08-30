using  ClinicFlow.Domain.Enums;

namespace ClinicFlow.Domain.Entities
{
    public class Invoice
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }

        public decimal SubTotal { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public PaymentStatusEnum Status { get; set; }

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        public Appointment Appointment { get; set; } = null!;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }




























}
