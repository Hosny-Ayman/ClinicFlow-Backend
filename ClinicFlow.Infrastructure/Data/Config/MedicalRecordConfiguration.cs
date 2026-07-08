using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.ToTable("MedicalRecords");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Diagnosis)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(m => m.TreatmentPlan)
                .IsRequired()
               .HasMaxLength(1000);

            builder.Property(m => m.Symptoms)
                .HasMaxLength(2000);

            builder.Property(m => m.Notes)
                .HasMaxLength(3000);

            builder.Property(m => m.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(m => m.PatientId);

            builder.HasIndex(m => m.DoctorId);

            builder.HasIndex(m => m.AppointmentId)
                .IsUnique();

            builder.HasIndex(m => m.CreatedAt);

            builder.HasIndex(m => new
            {
                m.PatientId,
                m.CreatedAt
            });

            builder.HasIndex(m => new
            {
                m.DoctorId,
                m.CreatedAt
            });

            builder.HasOne(m => m.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Doctor)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Appointment)
                .WithOne(a => a.MedicalRecord)
                .HasForeignKey<MedicalRecord>(m => m.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.Prescriptions)
                .WithOne(p => p.MedicalRecord)
                .HasForeignKey(p => p.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.Attachments)
                .WithOne(a => a.MedicalRecord)
                .HasForeignKey(a => a.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }













































}
