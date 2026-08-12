using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class ClinicPatientConfiguration : IEntityTypeConfiguration<ClinicPatient>
    {
        public void Configure(EntityTypeBuilder<ClinicPatient> builder)
        {
            builder.ToTable("ClinicPatients");

            builder.HasKey(cp => cp.Id);

            builder.Property(cp => cp.ClinicId).IsRequired();

            builder.Property(cp => cp.PatientId).IsRequired();

            builder.Property(cp => cp.FirstVisitDate);

            builder.Property(cp => cp.IsActive)
                .HasDefaultValue(true);

            builder.Property(cp => cp.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

           

            builder.HasIndex(cp => cp.ClinicId);

            builder.HasIndex(cp => cp.PatientId);

            builder.HasIndex(cp => cp.IsActive);

            builder.HasIndex(cp => cp.CreatedAt);

            builder.HasOne(cp => cp.Clinic)
                .WithMany(c => c.ClinicPatients)
                .HasForeignKey(cp => cp.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cp => cp.Patient)
                .WithMany(p => p.ClinicPatients)
                .HasForeignKey(cp => cp.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
