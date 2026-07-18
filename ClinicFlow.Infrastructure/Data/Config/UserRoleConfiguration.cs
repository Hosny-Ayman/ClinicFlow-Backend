using ClinicFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Data.Config
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            builder.HasKey(pm => pm.Id);



            builder.HasIndex(pm => pm.UserId);
            builder.HasIndex(pm => pm.RoleId);



            builder.HasOne(u => u.User)
                 .WithMany(r => r.UserRoles)
                 .HasForeignKey(r => r.UserId);

            builder.HasOne(u => u.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(r => r.RoleId);

          
        }
    }


}
