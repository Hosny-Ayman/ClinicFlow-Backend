using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("PaymentMethods");

            builder.HasKey(pm => pm.Id);

            builder.Property(pm => pm.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pm => pm.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(pm => pm.Name)
                .IsUnique();

            builder.HasIndex(pm => pm.IsActive);

            builder.HasMany(pm => pm.Payments)
                .WithOne(p => p.PaymentMethod)
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }


}
