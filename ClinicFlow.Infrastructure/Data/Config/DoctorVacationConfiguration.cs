using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class DoctorVacationConfiguration : IEntityTypeConfiguration<DoctorVacation>
    {
        public void Configure(EntityTypeBuilder<DoctorVacation> builder)
        {
            builder.ToTable("DoctorVacations");

            builder.HasKey(dv => dv.Id);

            builder.Property(dv => dv.StartDate)
                .IsRequired();

            builder.Property(dv => dv.EndDate)
                .IsRequired();

            builder.Property(dv => dv.Reason)
                .HasMaxLength(500);

            builder.HasIndex(dv => dv.DoctorId);

            builder.HasIndex(dv => dv.StartDate);

            builder.HasIndex(dv => dv.EndDate);

            builder.HasIndex(dv => new
            {
                dv.DoctorId,
                dv.StartDate,
                dv.EndDate
            });

            builder.HasOne(dv => dv.Doctor)
                .WithMany(d => d.DoctorVacations)
                .HasForeignKey(dv => dv.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
























}
