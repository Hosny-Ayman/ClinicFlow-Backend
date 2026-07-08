using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class ClinicConfigration : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {

            builder.ToTable("Clinics");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)  
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.LogoUrl)
                .HasMaxLength(500);

            builder.Property(c => c.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.Address)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(c => c.Description)
                .HasMaxLength(1000);

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(c => c.Name).IsUnique();

            builder.HasIndex(c => c.Email)
                .IsUnique();

            builder.HasIndex(c => c.Phone)
                .IsUnique();

            builder.HasIndex(c => c.CreatedAt);

            builder.HasIndex(c => c.IsActive);

            
            builder.HasMany(c => c.Users)
                .WithOne(u => u.Clinic)
                .HasForeignKey(u => u.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Doctors)
                .WithOne(d => d.Clinic)
                .HasForeignKey(d => d.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Patients)
                .WithOne(p => p.Clinic)
                .HasForeignKey(p => p.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Invoices)
                .WithOne()
                .HasForeignKey(i => i.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }












}
