using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("PaymentMethods");

            builder.HasKey(pm => pm.Id);

            builder.Property(pm => pm.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pm => pm.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(pm => pm.Name)
                .IsUnique();

            builder.HasIndex(pm => pm.IsActive);

            builder.HasMany(pm => pm.Payments)
                .WithOne(p => p.PaymentMethod)
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }





    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            builder.HasKey(pm => pm.Id);



            builder.HasIndex(pm => pm.UserId);
            builder.HasIndex(pm => pm.RoleId);
            builder.HasIndex(pm => pm.ClinicId);



            builder.HasOne(u => u.User)
                 .WithMany(r => r.UserRoles)
                 .HasForeignKey(r => r.UserId);

            builder.HasOne(u => u.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(r => r.RoleId);

            builder.HasOne(u => u.Clinic)
              .WithMany(r => r.UserRoles)
              .HasForeignKey(r => r.ClinicId);
        }
    }


}
