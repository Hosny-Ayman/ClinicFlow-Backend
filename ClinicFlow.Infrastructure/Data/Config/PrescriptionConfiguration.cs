using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.ToTable("Prescriptions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Notes)
                .HasMaxLength(2000);

            builder.Property(p => p.IssuedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(p => p.MedicalRecordId);

            builder.HasIndex(p => p.DoctorId);

            builder.HasIndex(p => p.IssuedAt);

            builder.HasIndex(p => new
            {
                p.DoctorId,
                p.IssuedAt
            });

            builder.HasIndex(p => new
            {
                p.MedicalRecordId,
                p.IssuedAt
            });

            builder.HasOne(p => p.MedicalRecord)
                .WithMany(m => m.Prescriptions)
                .HasForeignKey(p => p.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.PrescriptionItems)
                .WithOne(pi => pi.Prescription)
                .HasForeignKey(pi => pi.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }









































}
