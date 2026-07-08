using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.Email)
                .HasMaxLength(255);

            builder.Property(p => p.Notes)
                .IsRequired(false)
               .HasMaxLength(500);

            builder.Property(p => p.Gender).IsRequired();

            builder.Property(p => p.Address)
                .HasMaxLength(300);

            builder.Property(p => p.BloodType).IsRequired(false);

            builder.Property(p => p.NationalId).IsRequired(false).HasMaxLength(14);
               

            builder.Property(p => p.EmergencyContactName)
                .HasMaxLength(200);

            builder.Property(p => p.EmergencyContactPhone)
                .HasMaxLength(20);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(p => p.ClinicId);

            builder.HasIndex(p => p.PhoneNumber);

            builder.HasIndex(p => p.Email);

            builder.HasIndex(p => p.Gender);

            builder.HasIndex(p => p.FirstName);

            builder.HasIndex(p => p.LastName);

            builder.HasIndex(p => new
            {
                p.FirstName,
                p.LastName
            });

            builder.HasIndex(p => p.DateOfBirth);

            builder.HasIndex(p => p.Gender);

            builder.HasIndex(p => p.CreatedAt);

            builder.HasOne(p => p.Clinic)
                .WithMany(c => c.Patients)
                .HasForeignKey(p => p.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Appointments)
                .WithOne(a => a.Patient)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.MedicalRecords)
                .WithOne(m => m.Patient)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Invoices)
                .WithOne(i => i.Patient)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }




















}
