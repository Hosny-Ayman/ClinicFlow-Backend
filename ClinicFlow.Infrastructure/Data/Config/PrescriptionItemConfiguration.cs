using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
    {
        public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
        {
            builder.ToTable("PrescriptionItems");

            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.MedicationName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(pi => pi.Dosage)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pi => pi.Frequency)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pi => pi.Duration)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pi => pi.Instructions)
                .HasMaxLength(1000);

             
            builder.HasIndex(pi => pi.PrescriptionId);

            builder.HasIndex(pi => pi.MedicationName);

            builder.HasIndex(pi => new
            {
                pi.PrescriptionId,
                pi.MedicationName
            });

            builder.HasOne(pi => pi.Prescription)
                .WithMany(p => p.PrescriptionItems)
                .HasForeignKey(pi => pi.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }





































}
