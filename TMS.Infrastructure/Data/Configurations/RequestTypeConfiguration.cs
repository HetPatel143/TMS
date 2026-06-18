using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Data.Configurations
{
    public class RequestTypeConfiguration : IEntityTypeConfiguration<RequestType>
    {
        public void Configure(EntityTypeBuilder<RequestType> builder)
        {
            builder.ToTable("RequestTypes");
            builder.HasKey(rt => rt.RequestTypeId);
            builder.Property(rt => rt.RequestTypeId).UseIdentityColumn();

            builder.Property(rt => rt.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(rt => rt.Name).IsUnique();

            builder.Property(rt => rt.Description)
                .HasMaxLength(255);

            builder.Property(rt => rt.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(rt => rt.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(rt => rt.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(rt => rt.CreatedByUser)
                .WithMany(u => u.CreatedRequestTypes)
                .HasForeignKey(rt => rt.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed request types
            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            builder.HasData(
                new RequestType { RequestTypeId = 1, Name = "Laptop Request", Description = "Request for new laptop or laptop replacement", IsActive = true, CreatedBy = 1, CreatedAt = seedDate, UpdatedAt = seedDate },
                new RequestType { RequestTypeId = 2, Name = "Software Access Request", Description = "Request access to software applications", IsActive = true, CreatedBy = 1, CreatedAt = seedDate, UpdatedAt = seedDate },
                new RequestType { RequestTypeId = 3, Name = "Reimbursement Request", Description = "Expense reimbursement request", IsActive = true, CreatedBy = 1, CreatedAt = seedDate, UpdatedAt = seedDate },
                new RequestType { RequestTypeId = 4, Name = "Leave Request", Description = "Request for leave/time off", IsActive = true, CreatedBy = 1, CreatedAt = seedDate, UpdatedAt = seedDate },
                new RequestType { RequestTypeId = 5, Name = "WFH Request", Description = "Work from home request", IsActive = true, CreatedBy = 1, CreatedAt = seedDate, UpdatedAt = seedDate }
            );
        }
    }
}
