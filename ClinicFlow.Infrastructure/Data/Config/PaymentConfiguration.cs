using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.TransactionId)
                .HasMaxLength(255);

            builder.Property(p => p.PaidAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(p => p.InvoiceId);

            builder.HasIndex(p => p.PaymentMethodId);

            builder.HasIndex(p => p.Status);

            builder.HasIndex(p => p.PaidAt);

            builder.HasIndex(p => p.TransactionId)
                .IsUnique();

            builder.HasIndex(p => new
            {
                p.InvoiceId,
                p.Status
            });

            builder.HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.PaymentMethod)
                .WithMany(pm => pm.Payments)
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }



























}
