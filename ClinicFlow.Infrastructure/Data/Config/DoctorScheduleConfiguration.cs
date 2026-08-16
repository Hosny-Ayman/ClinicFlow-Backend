using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
    {
        public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
        {
            builder.ToTable("DoctorSchedules");

            builder.HasKey(ds => ds.Id);

            builder.Property(ds => ds.DayOfWeek)
                .IsRequired();

            builder.Property(ds => ds.StartTime)
                .IsRequired(false);

            builder.Property(ds => ds.EndTime)
                .IsRequired(false);

            builder.Property(ds => ds.IsAvailable)
                .HasDefaultValue(true);

            builder.HasIndex(ds => ds.DoctorId);

            builder.HasIndex(ds => ds.DayOfWeek);


            builder.HasIndex(ds => new
            {
                ds.DoctorId,
                ds.DayOfWeek
            });

            builder.HasIndex(ds => new
            {
                ds.DoctorId,
                ds.DayOfWeek,
                ds.StartTime,
                ds.EndTime
            });

            builder.HasOne(ds => ds.Doctor)
                .WithMany(d => d.DoctorSchedules)
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }





























}
