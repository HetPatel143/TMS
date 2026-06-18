using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Data.Configurations
{
    public class RequestApprovalConfiguration : IEntityTypeConfiguration<RequestApproval>
    {
        public void Configure(EntityTypeBuilder<RequestApproval> builder)
        {
            builder.ToTable("RequestApprovals");
            builder.HasKey(ra => ra.ApprovalId);
            builder.Property(ra => ra.ApprovalId).UseIdentityColumn();

            builder.Property(ra => ra.ApprovalOrder)
                .IsRequired();

            builder.Property(ra => ra.ApprovalStatus)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(ra => ra.Comments)
                .HasColumnType("nvarchar(max)");

            builder.Property(ra => ra.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(ra => ra.RowVersion)
                .IsRowVersion();

            builder.HasOne(ra => ra.Request)
                .WithMany(r => r.Approvals)
                .HasForeignKey(ra => ra.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ra => ra.Role)
                .WithMany(r => r.RequestApprovals)
                .HasForeignKey(ra => ra.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ra => ra.ApprovedByUser)
                .WithMany(u => u.RequestApprovals)
                .HasForeignKey(ra => ra.ApprovedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
