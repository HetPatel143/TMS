using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.UserId);
            builder.Property(u => u.UserId).UseIdentityColumn();

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed demo users - All passwords are BCrypt hashes of "Password@123" (except Admin which uses "Admin@123")
            builder.HasData(
                // Admin
                new User
                {
                    UserId = 1,
                    FirstName = "System",
                    LastName = "Admin",
                    Email = "admin@tms.com",
                    PasswordHash = "$2a$11$xNTPybxPn.NdKbbK9CB2Ge8FVoTlaVbxNNLzWpXYNouY7MnD3mWB2", // Admin@123
                    RoleId = 6,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                // Employee 1
                new User
                {
                    UserId = 2,
                    FirstName = "Rahul",
                    LastName = "Sharma",
                    Email = "rahul.sharma@tms.com",
                    PasswordHash = "$2a$11$tZUQUcvbLH5xDr6mk99fD.KnrKkF3piGTY8UuuSVGQKTDPN9WI3b6", // Password@123
                    RoleId = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                // Employee 2
                new User
                {
                    UserId = 3,
                    FirstName = "Priya",
                    LastName = "Patel",
                    Email = "priya.patel@tms.com",
                    PasswordHash = "$2a$11$yuecjY2kxPzGZB.fB4f3KurxIhzg4zvu/I.r8wpOeN2SkfRVEXxcC", // Password@123
                    RoleId = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                // Manager
                new User
                {
                    UserId = 4,
                    FirstName = "Vikram",
                    LastName = "Singh",
                    Email = "vikram.singh@tms.com",
                    PasswordHash = "$2a$11$ZZJNUHcuBYQlGcm5ytswi.247d7dAQ8KBu9ulgqQ4wncFnHKgoPJS", // Password@123
                    RoleId = 2,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                // Finance
                new User
                {
                    UserId = 5,
                    FirstName = "Anita",
                    LastName = "Desai",
                    Email = "anita.desai@tms.com",
                    PasswordHash = "$2a$11$5NcADmc3A.VXjtHIP5Tw9.eXlu5O0hsMyJ/QRTmkPP6O5vdzWlae.", // Password@123
                    RoleId = 3,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                // IT
                new User
                {
                    UserId = 6,
                    FirstName = "Rajesh",
                    LastName = "Kumar",
                    Email = "rajesh.kumar@tms.com",
                    PasswordHash = "$2a$11$Sb4gn6BFeFYB9B6DxxyZHORJlXzfvfXyDHoUcr3H4o.z0xcCx60VO", // Password@123
                    RoleId = 4,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                // HR
                new User
                {
                    UserId = 7,
                    FirstName = "Sneha",
                    LastName = "Gupta",
                    Email = "sneha.gupta@tms.com",
                    PasswordHash = "$2a$11$6prRBVopmQ3VIvud1zvAYutFVBweVWvNTSRPv8lEMjgraRMbigGJu", // Password@123
                    RoleId = 5,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
