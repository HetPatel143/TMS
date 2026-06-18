using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Data.Configurations
{
    public class RequestConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.ToTable("Requests");
            builder.HasKey(r => r.RequestId);
            builder.Property(r => r.RequestId).UseIdentityColumn();

            builder.Property(r => r.RequestNumber)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(r => r.RequestNumber)
                .IsUnique();

            builder.Property(r => r.CurrentStatus)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(r => r.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(r => r.RequestType)
                .WithMany(rt => rt.Requests)
                .HasForeignKey(r => r.RequestTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.CreatedByUser)
                .WithMany(u => u.CreatedRequests)
                .HasForeignKey(r => r.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
