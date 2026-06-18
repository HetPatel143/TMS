using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(r => r.RoleId);
            builder.Property(r => r.RoleId).UseIdentityColumn();

            builder.Property(r => r.RoleName)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => r.RoleName).IsUnique();

            builder.Property(r => r.Description)
                .HasMaxLength(255);

            builder.Property(r => r.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Seed data
            builder.HasData(
                new Role { RoleId = 1, RoleName = "Employee", Description = "Regular employee", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Role { RoleId = 2, RoleName = "Manager", Description = "Team manager", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Role { RoleId = 3, RoleName = "Finance", Description = "Finance department", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Role { RoleId = 4, RoleName = "IT", Description = "IT department", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Role { RoleId = 5, RoleName = "HR", Description = "Human resources", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Role { RoleId = 6, RoleName = "Admin", Description = "System administrator", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
