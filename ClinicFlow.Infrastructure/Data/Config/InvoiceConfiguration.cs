using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.SubTotal)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(i => i.DiscountAmount)
                .HasPrecision(18, 2);

            builder.Property(i => i.TotalAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(i => i.Status)
                .IsRequired()
                .IsRequired();

            builder.Property(i => i.IssuedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(i => i.ClinicId);

            builder.HasIndex(i => i.PatientId);

            builder.HasIndex(i => i.AppointmentId)
                .IsUnique();

            builder.HasIndex(i => i.Status);

            builder.HasIndex(i => i.IssuedAt);

            builder.HasIndex(i => new
            {
                i.ClinicId,
                i.Status
            });

            builder.HasIndex(i => new
            {
                i.PatientId,
                i.IssuedAt
            });

            builder.HasOne(i => i.Clinic)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Patient)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Appointment)
                .WithOne(a => a.Invoice)
                .HasForeignKey<Invoice>(i => i.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.Payments)
                .WithOne(p => p.Invoice)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
































}
