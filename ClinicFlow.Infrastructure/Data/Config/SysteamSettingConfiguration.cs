using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class SysteamSettingConfiguration : IEntityTypeConfiguration<SysteamSetting>
    {
        public void Configure(EntityTypeBuilder<SysteamSetting> builder)
        {
            builder.ToTable("SysteamSettings");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.SettingKey)
                .IsRequired()               
                .HasMaxLength(255);

            builder.Property(a => a.SettingValue)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(a => a.Description)
                .IsRequired(false)
                .HasMaxLength(1000);



            builder.HasIndex(a => a.SettingKey).IsUnique(true);


        }
    }
}
