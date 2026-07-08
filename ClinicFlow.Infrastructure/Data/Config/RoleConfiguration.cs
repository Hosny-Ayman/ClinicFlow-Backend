using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Description)
              .IsRequired(false)
              .HasMaxLength(500);

            builder.Property(r => r.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(pm => pm.Name)
                .IsUnique();

            builder.HasIndex(pm => pm.IsActive);

            builder.HasMany(r => r.UserRoles)
                 .WithOne(u => u.Role)
                 .HasForeignKey(r => r.RoleId);
        }
    }

















}
