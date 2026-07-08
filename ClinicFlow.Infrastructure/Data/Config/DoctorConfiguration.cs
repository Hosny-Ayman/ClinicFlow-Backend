using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("Doctors");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.ConsultationFee)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(d => d.Bio)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(d => d.ExperienceYears).IsRequired();

            builder.Property(u => u.ProfileImageUrl)
              .IsRequired()
              .HasMaxLength(500);

            builder.HasIndex(d => d.UserId)
                .IsUnique();

            builder.HasIndex(d => d.ClinicId);

            builder.HasIndex(d => d.SpecialtyId);

            builder.HasIndex(d => d.ExperienceYears);

            builder.HasIndex(d => d.ConsultationFee);

            builder.HasIndex(d => new { d.ClinicId, d.SpecialtyId });

            builder.HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Clinic)
                .WithMany(c => c.Doctors)
                .HasForeignKey(d => d.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Specialty)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.DoctorSchedules)
                .WithOne(ds => ds.Doctor)
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.DoctorVacations)
                .WithOne(dv => dv.Doctor)
                .HasForeignKey(dv => dv.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Appointments)
                .WithOne(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.Prescriptions)
                .WithOne(p => p.Doctor)
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.MedicalRecords)
                .WithOne(m => m.Doctor)
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }







































}
